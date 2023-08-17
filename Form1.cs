namespace RHTableTool
{
    public partial class Form1 : Form
    {
        private readonly List<string> fileList = new();
        private bool isEncrypt = false;
        private bool runing = false;
        private Thread? thread;
        private CancellationTokenSource? cancellationTokenSource;
        private CryptType cType = CryptType.XLSX;
        private bool repSting2 = false;

        public Form1()
        {
            InitializeComponent();
            cbxType.SelectedIndex = (int)cType;
        }

        static public void OpenOutputFolder()
        {
            string dirOutput = Path.Combine(Environment.CurrentDirectory, "Output");
            if (!Directory.Exists(dirOutput)) Directory.CreateDirectory(dirOutput);
            OpenFolderAndSelectFile(dirOutput, false);
        }

        static public void OpenFolderAndSelectFile(string fileFullName, bool select)
        {
            string strSelect = select ? "/select," : "";
            string arguments = string.Format("/e,{0}\"{1}\"", strSelect, fileFullName);
            System.Diagnostics.Process.Start("Explorer.exe", arguments);
        }

        private void btnOpenDir_Click(object sender, EventArgs e)
        {
            OpenOutputFolder();
        }

        public delegate void WriteTxtDelegate(string txt);
        public void WriteTxt(string txt)
        {
            if (txtFile.InvokeRequired) txtFile.Invoke(new WriteTxtDelegate(WriteTxt), txt);
            else txtFile.AppendText(txt);
        }

        private void RuningThread(CancellationToken cancellationToken)
        {
            while (fileList.Count > 0 && !cancellationToken.IsCancellationRequested)
            {
                string path = fileList[0];
                FileDecryptEncrypt(path);
                fileList.RemoveAt(0);
            }
            runing = false;
            thread = null;
        }

        private void RunCrypt()
        {
            if (!runing)
            {
                cancellationTokenSource = new CancellationTokenSource();
                thread = new Thread(() => RuningThread(cancellationTokenSource.Token));
                repSting2 = cbxRepString2.Checked;
                isEncrypt = rbtnEn.Checked;
                cType = (CryptType)cbxType.SelectedIndex;
                thread.Start();
            }
        }


        public void FileDecryptEncrypt(string filePath)
        {
            byte[] bytes2 = null;
            if (File.Exists(filePath) || Directory.Exists(filePath))
            {
                WriteTxt((isEncrypt ? "\r\nEncrypting：" : "\r\nDecrypting：") + filePath + "\r\n");

                if (File.GetAttributes(filePath).HasFlag(FileAttributes.Directory))
                {
                    string outputFolder = "Output\\" + new DirectoryInfo(filePath).Name;
                    if (!Directory.Exists(outputFolder)) Directory.CreateDirectory(outputFolder);
                    foreach (string file in Directory.GetFiles(filePath))
                    {
                        string fileName = (new FileInfo(file)).Name;
                        if (isEncrypt) fileName = fileName[..fileName.LastIndexOf('.')];

                        try
                        {
                            switch (cType)
                            {
                                case CryptType.XLSX:
                                    {
                                        if (isEncrypt)
                                            bytes2 = XLSXCrypt.XLSXToRh(File.ReadAllBytes(file));
                                        else
                                        {
                                            bytes2 = XLSXCrypt.RhToXLSX(File.ReadAllBytes(file), repSting2);
                                            fileName += ".xlsx";
                                        }
                                    }
                                    break;
                                case CryptType.XML:
                                    {
                                        if (isEncrypt)
                                            bytes2 = XMLCrypt.XMLToRh(File.ReadAllBytes(file));
                                        else
                                        {
                                            bytes2 = XMLCrypt.RhToXML(File.ReadAllBytes(file), repSting2);
                                            fileName += ".xml";
                                        }
                                    }
                                    break;
                            }

                            string outputFile = outputFolder + "\\" + fileName;
                            if (!Directory.Exists(outputFolder + "\\" + cType.ToString())) // create the subfolder if it doesn't exist
                                Directory.CreateDirectory(outputFolder + "\\" + cType.ToString());
                            File.WriteAllBytes(outputFolder + "\\" + cType.ToString() + "\\" + fileName, bytes2);
                            WriteTxt("File saved in: " + outputFolder + "\\" + cType.ToString() + "\\" + fileName + "\r\n");
                        }
                        catch (Exception ex)
                        {
                            WriteTxt("Error：" + ex.Message + "\r\n");
                            return;
                        }
                    }
                }
                else
                {
                    byte[] bytes1 = File.ReadAllBytes(filePath);

                    string fileName = (new FileInfo(filePath)).Name;
                    if (isEncrypt) fileName = fileName[..fileName.LastIndexOf('.')];

                    try
                    {
                        switch (cType)
                        {
                            case CryptType.XLSX:
                                {
                                    if (isEncrypt)
                                        bytes2 = XLSXCrypt.XLSXToRh(bytes1);
                                    else
                                    {
                                        bytes2 = XLSXCrypt.RhToXLSX(bytes1, repSting2);
                                        fileName += ".xlsx";
                                    }
                                }
                                break;
                            case CryptType.XML:
                                {
                                    if (isEncrypt)
                                        bytes2 = XMLCrypt.XMLToRh(bytes1);
                                    else
                                    {
                                        bytes2 = XMLCrypt.RhToXML(bytes1, repSting2);
                                        fileName += ".xml";
                                    }
                                }
                                break;
                        }

                        filePath = "Output\\" + fileName;
                        if (!Directory.Exists("Output")) Directory.CreateDirectory("Output");
                        if (!Directory.Exists("Output\\" + cType.ToString())) // create the subfolder if it doesn't exist
                            Directory.CreateDirectory("Output\\" + cType.ToString());
                        File.WriteAllBytes("Output\\" + cType.ToString() + "\\" + fileName, bytes2);
                        WriteTxt("File saved in: " + "Output\\" + cType.ToString() + "\\" + fileName + "\r\n");
                    }
                    catch (Exception ex)
                    {
                        WriteTxt("Error：" + ex.Message + "\r\n");
                        return;
                    }
                }
            }
            else WriteTxt("\r\nThe file or directory does not exist：" + filePath + "\r\n");
        }

        private void txtFile_DragDrop(object sender, DragEventArgs e)
        {
            Array aryFiles = (Array)e.Data.GetData(DataFormats.FileDrop);
            int len = aryFiles.Length;
            for (int i = 0; i < len; i++)
                fileList.Add(aryFiles.GetValue(i).ToString());

            RunCrypt();
        }

        private void txtFile_DragEnter(object sender, DragEventArgs e)
        {
            DragDropEffects ee = DragDropEffects.None;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                Array aryFiles = (Array)e.Data.GetData(DataFormats.FileDrop);

                if (aryFiles.GetType().Equals(typeof(string[])))
                {
                    int len = aryFiles.Length;
                    if (len > 0)
                    {
                        bool canMove = true;
                        for (int i = 0; i < len; i++)
                        {
                            string path = (string)aryFiles.GetValue(i);
                            if (!File.Exists(path) && !Directory.Exists(path))
                            {
                                canMove = false;
                                break;
                            }
                        }

                        if (canMove) ee = DragDropEffects.Move;
                    }
                }
            }
            e.Effect = ee;
        }

        private string selectedFolderPath;

        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog folderBrowserDialog = new();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFolderPath = folderBrowserDialog.SelectedPath;
                tbBrowseFolder.Text = selectedFolderPath;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedFolderPath))
            {
                RunCrypt(selectedFolderPath);
            }
            else
            {
                MessageBox.Show("Please select a folder before starting.", "Folder Not Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RunCrypt(string folderPath)
        {
            if (!runing)
            {
                cancellationTokenSource = new CancellationTokenSource();
                thread = new Thread(() => RuningThread(cancellationTokenSource.Token, folderPath));
                repSting2 = cbxRepString2.Checked;
                isEncrypt = rbtnEn.Checked;
                cType = (CryptType)cbxType.SelectedIndex;
                thread.Start();
            }
        }

        private void RuningThread(CancellationToken cancellationToken, string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath);

            foreach (string path in files)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                FileDecryptEncrypt(path);
            }

            runing = false;
            thread = null;
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            txtFile.Clear();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            cancellationTokenSource?.Cancel();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                fileList.Clear();
                thread = null;
                runing = false;
            }
        }


    }
}
