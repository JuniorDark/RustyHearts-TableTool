using System.Diagnostics;
using RHTableTool.Cryptor;

namespace RHTableTool
{
    public partial class MainForm : Form
    {
        public string Url = "https://github.com/JuniorDark/RustyHearts-TableTool";

        #region Configuration Variables
        private CryptType cType = CryptType.XLSX;
        private bool isEncrypt = false;
        private bool repSting2 = false;
        private string? inputFolderPath;
        private string? selectedInputFolderPath;
        private string? selectedOutputFolderPath;
        private readonly List<string> fileList = new();
        private CancellationTokenSource? cancellationTokenSource;
        private int processedFiles = 0;
        private int skippedFiles = 0;
        private int warningFiles = 0;
        private int totalFiles = 0;
        private int errorFiles = 0;
        #endregion

        public MainForm()
        {
            InitializeComponent();
            cbxType.SelectedIndex = (int)cType;
            btnStop.Enabled = false;
            lbVersion.Text = $"Version: {GetVersion()}";

        }

        #region Form Event Handlers

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadConfiguration();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cancellationTokenSource?.Cancel();
            SaveConfiguration();
        }

        #endregion

        #region Configuration
        private const string ConfigFileName = "Config.ini";

        private void LoadConfiguration()
        {
            try
            {
                IniFile ini = new(ConfigFileName);
                tbInputFolder.Text = ini.ReadValue("InputFolder", "");
                tbOutputFolder.Text = ini.ReadValue("OutputFolder", "Output");
                cbxType.Text = ini.ReadValue("Type", "");

                selectedInputFolderPath = tbInputFolder.Text;
                selectedOutputFolderPath = tbOutputFolder.Text;

                if (!string.IsNullOrEmpty(selectedInputFolderPath))
                {
                    btnOpenInputFolder.Enabled = true;
                }
                else
                {
                    btnOpenInputFolder.Enabled = false;
                }

                if (!string.IsNullOrEmpty(selectedOutputFolderPath))
                {
                    btnOpenOutputFolder.Enabled = true;
                }

                // Load radio button state
                string encryptionType = ini.ReadValue("EncryptionType", "");
                if (!string.IsNullOrEmpty(encryptionType))
                {
                    if (encryptionType == "Encrypt")
                    {
                        rbtnEncrypt.Checked = true;
                    }
                    else if (encryptionType == "Decrypt")
                    {
                        rbtnDecrypt.Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading configuration: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveConfiguration()
        {
            try
            {
                IniFile ini = new(ConfigFileName);
                ini.WriteValue("InputFolder", tbInputFolder.Text);
                ini.WriteValue("OutputFolder", tbOutputFolder.Text);
                ini.WriteValue("Type", cbxType.Text);

                // Save radio button state
                if (rbtnEncrypt.Checked)
                {
                    ini.WriteValue("EncryptionType", "Encrypt");
                }
                else if (rbtnDecrypt.Checked)
                {
                    ini.WriteValue("EncryptionType", "Decrypt");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving configuration: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Cryptor
        private async Task ProcessFiles(IEnumerable<string> files)
        {
            InitializeProcessing();
            totalFiles = files.Count();

            try
            {
                await Task.Run(() =>
                {
                    foreach (string path in files)
                    {
                        if (cancellationTokenSource != null)
                        {
                            cancellationTokenSource.Token.ThrowIfCancellationRequested();
                            FileDecryptEncrypt(path, cancellationTokenSource.Token);
                        }
                    }
                }, cancellationTokenSource?.Token ?? CancellationToken.None);
            }
            catch (OperationCanceledException)
            {
                WriteTxt("\r\nOperation was canceled.\r\n");
            }
            finally
            {
                FinalizeProcessing();
            }
        }

        private void FileDecryptEncrypt(string filePath, CancellationToken cancellationToken)
        {
            if (!File.Exists(filePath) && !Directory.Exists(filePath))
            {
                WriteTxt($"\r\nThe file or directory does not exist: {filePath}\r\n");
                return;
            }

            string operation = isEncrypt ? "Encrypting" : "Decrypting";
            string fileName = Path.GetFileName(filePath);
            Interlocked.Increment(ref processedFiles);
            WriteTxt($"\r\n{operation}: {fileName} ({processedFiles}/{totalFiles})\r\n");

            if (File.GetAttributes(filePath).HasFlag(FileAttributes.Directory))
            {
                foreach (string file in Directory.GetFiles(filePath))
                {
                    ProcessFile(file, cancellationToken);
                }
            }
            else
            {
                ProcessFile(filePath, cancellationToken);
            }
        }

        private void ProcessFile(string sourceFile, CancellationToken cancellationToken)
        {
            string fileName = Path.GetFileName(sourceFile);
            string fileExtension = Path.GetExtension(sourceFile).ToLower();
            bool isRhFile = fileExtension == ".rh";
            bool isXLSXFile = fileExtension == ".xlsx";
            bool isXMLFile = fileExtension == ".xml";

            try
            {
                //Event Handlers
                XMLCryptor.WarningLogged += LogWarning;
                XMLV2Cryptor.WarningLogged += LogWarning;
                XLSXCryptor.WarningLogged += LogWarning;

                if (isEncrypt)
                {
                    if (cType == CryptType.XLSX && !isXLSXFile)
                    {
                        string errorMessage = $"Encryption Error: File {fileName} skipped: Only .xlsx files can be encrypted with the selected File Type (XLSX).\r\n";
                        WriteTxt(errorMessage);
                        LogErrorToFile("error.log", fileName, errorMessage);
                        Interlocked.Increment(ref skippedFiles);
                        return;
                    }
                    else if ((cType == CryptType.XML || cType == CryptType.XMLV2) && !isXMLFile)
                    {
                        string errorMessage = $"Encryption Error: File {fileName} skipped: Only .xml files can be encrypted with the selected File Type (XML or XMLV2).\r\n";
                        WriteTxt(errorMessage);
                        LogErrorToFile("error.log", fileName, errorMessage);
                        Interlocked.Increment(ref skippedFiles);
                        return;
                    }
                }
                else if (!isRhFile)
                {
                    string errorMessage = $"Decryption Error: File {fileName} skipped: Only .rh files can be decrypted.\r\n";
                    WriteTxt(errorMessage);
                    LogErrorToFile("error.log", fileName, errorMessage);
                    Interlocked.Increment(ref skippedFiles);
                    return;
                }

                using FileStream sourceFileStream = File.OpenRead(sourceFile);
                byte[] buffer = new byte[4096];
                int bytesRead;

                using MemoryStream memoryStream = new();
                while ((bytesRead = sourceFileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    memoryStream.Write(buffer, 0, bytesRead);
                }

                byte[] sourceBytes = memoryStream.ToArray();

                byte[]? processedBytes = null;

                if (isEncrypt) fileName = fileName[..fileName.LastIndexOf('.')];

                switch (cType)
                {
                    case CryptType.XLSX:
                        if (isEncrypt)
                            processedBytes = XLSXCryptor.XLSXToRh(sourceBytes, sourceFile);
                        else
                        {
                            processedBytes = XLSXCryptor.RhToXLSX(sourceBytes, repSting2);
                            fileName += ".xlsx";
                        }
                        break;
                    case CryptType.XML:
                        if (isEncrypt)
                            processedBytes = XMLCryptor.XMLToRh(sourceBytes, sourceFile);
                        else
                        {
                            processedBytes = XMLCryptor.RhToXML(sourceBytes, repSting2);
                            fileName += ".xml";
                        }
                        break;
                    case CryptType.XMLV2:
                        if (isEncrypt)
                            processedBytes = XMLV2Cryptor.XMLV2ToRh(sourceBytes, sourceFile);
                        else
                        {
                            processedBytes = XMLV2Cryptor.RhToXMLV2(sourceBytes, repSting2);
                            fileName += ".xml";
                        }
                        break;
                }
                try
                {
                    if (processedBytes != null)
                    {
                        string outputFolder = selectedOutputFolderPath ?? "Output";

                        string sourceFileDirectory = Path.GetDirectoryName(sourceFile)!;
                        string relativePath = Path.GetRelativePath(inputFolderPath ?? string.Empty, sourceFileDirectory);
                        string convertionType = (isEncrypt ? "RH" : "Converted");

                        string cTypeFolder = Path.Combine(outputFolder, cType.ToString(), convertionType, relativePath);

                        if (!Directory.Exists(cTypeFolder))
                        {
                            Directory.CreateDirectory(cTypeFolder);
                        }

                        // Write processed data back to file
                        string outputFile = Path.Combine(cTypeFolder, fileName);
                        WriteToFile(outputFile, processedBytes, cancellationToken);
                        WriteTxt("File saved in: " + outputFile + "\r\n");
                    }
                }
                catch (Exception ex)
                {
                    string errorMessage = $"{(isEncrypt ? "Encryption" : "Decryption")} Error: process failed for the file {fileName}: {ex.Message}\r\n";
                    WriteTxt(errorMessage);
                    LogErrorToFile("error.log", fileName, errorMessage);
                    Interlocked.Increment(ref errorFiles);
                }
                finally
                {
                    // Unsubscribe from the events
                    XMLCryptor.WarningLogged -= LogWarning;
                    XMLV2Cryptor.WarningLogged -= LogWarning;
                    XLSXCryptor.WarningLogged -= LogWarning;
                }
            }
            catch (IOException ioEx)
            {
                string errorMessage = $"{(isEncrypt ? "Encryption" : "Decryption")} IO Error: {ioEx.Message}\r\n";
                WriteTxt(errorMessage);
                LogErrorToFile("error.log", fileName, errorMessage);
                Interlocked.Increment(ref errorFiles);
            }
            catch (Exception ex)
            {
                string errorMessage = "Error: " + ex.Message + "\r\n";
                WriteTxt(errorMessage);
                LogErrorToFile("error.log", fileName, errorMessage);
                Interlocked.Increment(ref errorFiles);
            }
        }

        private void WriteToFile(string filePath, byte[] data, CancellationToken cancellationToken)
        {
            try
            {
                using FileStream fileStream = File.Create(filePath);
                int bytesWritten = 0;
                int chunkSize = 4096;

                while (bytesWritten < data.Length)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    int remainingBytes = data.Length - bytesWritten;
                    int currentChunkSize = Math.Min(chunkSize, remainingBytes);

                    fileStream.Write(data, bytesWritten, currentChunkSize);
                    bytesWritten += currentChunkSize;
                }
            }
            catch (IOException ioEx)
            {
                string errorMessage = $"IO Exception occurred while writing to {filePath}: {ioEx.Message}";
                WriteTxt(errorMessage);
                LogErrorToFile("error", filePath, errorMessage);
            }
            catch (OperationCanceledException)
            {
                WriteTxt("\r\nOperation was canceled.\r\n");
            }
            catch (Exception ex)
            {
                string errorMessage = $"An error occurred while writing to {filePath}: {ex.Message}";
                WriteTxt(errorMessage);
                LogErrorToFile("error", filePath, errorMessage);
            }
        }
        #endregion

        #region Button Events
        private async void BtnStart_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedInputFolderPath))
            {
                txtFile.Clear();
                inputFolderPath = selectedInputFolderPath;
                await ProcessFiles(Directory.EnumerateFiles(selectedInputFolderPath, "*", SearchOption.AllDirectories));
            }
            else
            {
                MessageBox.Show("Please select a folder before starting.", "Folder Not Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            cancellationTokenSource?.Cancel();
        }

        private void BtnInputFolder_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog folderBrowserDialog = new();
            selectedInputFolderPath = string.Empty;

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                selectedInputFolderPath = folderBrowserDialog.SelectedPath;
                tbInputFolder.Text = selectedInputFolderPath;
            }
        }

        private void BtnOutputFolder_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog folderBrowserDialog = new();
            selectedOutputFolderPath = string.Empty;

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                selectedOutputFolderPath = folderBrowserDialog.SelectedPath;
                tbOutputFolder.Text = selectedOutputFolderPath;
            }
        }

        private void BtnOpenInputDir_Click(object sender, EventArgs e)
        {
            OpenInputFolder();
        }

        private void BtnOpenOutputDir_Click(object sender, EventArgs e)
        {
            OpenOutputFolder();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtFile.Clear();
        }

        private void LbVersion_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = Url, UseShellExecute = true });
        }

        #endregion

        #region Controls Events
        private async void TxtFile_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data != null)
            {
                string[]? items = e.Data.GetData(DataFormats.FileDrop) as string[];

                if (items?.Length > 0)
                {
                    txtFile.Clear();
                    List<string> filesToProcess = new();

                    inputFolderPath = Path.GetDirectoryName(items[0]);

                    foreach (string item in items)
                    {
                        if (File.Exists(item))
                        {
                            filesToProcess.Add(item);
                        }
                        else if (Directory.Exists(item))
                        {
                            filesToProcess.AddRange(Directory.EnumerateFiles(item, "*", SearchOption.AllDirectories));
                        }
                    }

                    if (filesToProcess.Count > 0)
                    {
                        txtFile.AllowDrop = false;

                        await ProcessFiles(filesToProcess);

                        txtFile.AllowDrop = true;
                    }
                }
            }
        }

        private void TxtFile_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data?.GetDataPresent(DataFormats.FileDrop) == true ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void TbInputFolder_TextChanged(object sender, EventArgs e)
        {
            string inputDir = tbInputFolder.Text;

            if (!Directory.Exists(inputDir))
            {
                tbInputFolder.Clear();
                selectedInputFolderPath = string.Empty;
                btnOpenInputFolder.Enabled = false;
            }
            else
            {
                btnOpenInputFolder.Enabled = true;
                selectedInputFolderPath = inputDir;
            }
        }

        private void TbOutputFolder_TextChanged(object sender, EventArgs e)
        {
            string outputDir = tbOutputFolder.Text;

            if (!Directory.Exists(outputDir))
            {
                tbOutputFolder.Text = "Output";
                selectedOutputFolderPath = tbOutputFolder.Text;
                btnOpenOutputFolder.Enabled = true;
            }
        }
        #endregion

        #region Methods
        private void InitializeProcessing()
        {
            processedFiles = 0;
            skippedFiles = 0;
            warningFiles = 0;
            errorFiles = 0;
            cType = (CryptType)cbxType.SelectedIndex;
            repSting2 = cbxRepString2.Checked;
            isEncrypt = rbtnEncrypt.Checked;
            btnStart.Enabled = false;
            btnStop.Enabled = true;

            cancellationTokenSource = new CancellationTokenSource();
        }

        private void FinalizeProcessing()
        {
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            fileList.Clear();

            if (totalFiles > 0)
            {
                string resultMessage = GetOperationResultMessage();
                WriteTxt(resultMessage);
                Interlocked.Exchange(ref warningFiles, 0);
            }
        }

        private string GetOperationResultMessage()
        {
            string resultMessage = "\r\nOperation complete. Total files: {0}, Processed files: {1}";

            if (errorFiles > 0 || skippedFiles > 0 || warningFiles > 0)
            {
                resultMessage += ", Error files: {4}, Warnings: {3}\r\n";
                resultMessage += "{4} file(s) failed to be processed/{2} file(s) skipped, {3} warnings, check the logs for more info.\r\n";
            }
            else if (errorFiles == 0 && skippedFiles == 0 && warningFiles > 0)
            {
                resultMessage += ", Warnings: {3}\r\n";
                resultMessage += "{3} warnings, check the warn log for more info.\r\n";
            }
            else
            {
                resultMessage += "\r\n";
            }

            return string.Format(resultMessage, totalFiles, processedFiles, skippedFiles, warningFiles, errorFiles);
        }

        private void OpenInputFolder()
        {
            string? inputFolder = selectedInputFolderPath;

            if (inputFolder != null && Directory.Exists(inputFolder))
            {
                OpenFolder(inputFolder);
            }
        }

        private void OpenOutputFolder()
        {
            string? outputFolder = selectedOutputFolderPath;

            if (outputFolder != null && Directory.Exists(outputFolder))
            {
                OpenFolder(outputFolder);
            }
            else
            {
                string dirOutput = Path.Combine(Environment.CurrentDirectory, "Output");

                if (!Directory.Exists(dirOutput))
                {
                    Directory.CreateDirectory(dirOutput);
                }

                OpenFolder(dirOutput);
            }
        }

        public static void OpenFolder(string directory)
        {
            if (!string.IsNullOrEmpty(directory))
            {
                Process.Start("explorer.exe", directory);
            }
        }

        public static string GetVersion()
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Application.ExecutablePath);

            string version = $"{versionInfo.FileMajorPart}.{versionInfo.FileMinorPart}.{versionInfo.FileBuildPart}";

            return version;
        }

        public void WriteTxt(string format, params object[] args)
        {
            string text = string.Format(format, args);

            if (txtFile.InvokeRequired)
            {
                txtFile.Invoke(new Action(() => txtFile.AppendText(text)));
            }
            else
            {
                txtFile.AppendText(text);
            }
        }

        private void LogWarning(string sourceFile, string message)
        {
            string fileName = Path.GetFileName(sourceFile);
            LogErrorToFile("warn.log", fileName, message);
            Interlocked.Increment(ref warningFiles);
        }

        public static void LogErrorToFile(string logFile, string fileName, string errorMessage)
        {
            string logFilePath = logFile;
            string date = DateTime.Now.ToString();
            using StreamWriter logWriter = new(logFilePath, true);
            logWriter.WriteLine($"[{date}]: {fileName}: {errorMessage}");
            logWriter.Flush();
        }

        #endregion
    }

}
