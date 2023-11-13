namespace RHTableTool
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            txtFile = new TextBox();
            gbCryptor = new GroupBox();
            pbString2Help = new PictureBox();
            btnStart = new Button();
            btnStop = new Button();
            btnClear = new Button();
            cbxRepString2 = new CheckBox();
            cbxType = new ComboBox();
            lbFileType = new Label();
            rbtnDecrypt = new RadioButton();
            rbtnEncrypt = new RadioButton();
            toolTip = new ToolTip(components);
            lbVersion = new Label();
            btnOpenInputFolder = new Button();
            btnOpenOutputFolder = new Button();
            pbHelp = new PictureBox();
            btnInputFolder = new Button();
            tbInputFolder = new TextBox();
            lbInputFolder = new Label();
            lbOutputFolder = new Label();
            tbOutputFolder = new TextBox();
            btnOutputFolder = new Button();
            gbCryptor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbString2Help).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbHelp).BeginInit();
            SuspendLayout();
            // 
            // txtFile
            // 
            txtFile.AllowDrop = true;
            txtFile.Location = new Point(14, 104);
            txtFile.Multiline = true;
            txtFile.Name = "txtFile";
            txtFile.ReadOnly = true;
            txtFile.ScrollBars = ScrollBars.Both;
            txtFile.Size = new Size(562, 234);
            txtFile.TabIndex = 0;
            txtFile.WordWrap = false;
            txtFile.DragDrop += TxtFile_DragDrop;
            txtFile.DragEnter += TxtFile_DragEnter;
            // 
            // gbCryptor
            // 
            gbCryptor.Controls.Add(pbString2Help);
            gbCryptor.Controls.Add(btnStart);
            gbCryptor.Controls.Add(btnStop);
            gbCryptor.Controls.Add(btnClear);
            gbCryptor.Controls.Add(cbxRepString2);
            gbCryptor.Controls.Add(cbxType);
            gbCryptor.Controls.Add(lbFileType);
            gbCryptor.Controls.Add(rbtnDecrypt);
            gbCryptor.Controls.Add(rbtnEncrypt);
            gbCryptor.Location = new Point(14, 344);
            gbCryptor.Name = "gbCryptor";
            gbCryptor.Size = new Size(562, 104);
            gbCryptor.TabIndex = 1;
            gbCryptor.TabStop = false;
            gbCryptor.Text = "File Processing";
            // 
            // pbString2Help
            // 
            pbString2Help.Cursor = Cursors.Help;
            pbString2Help.Image = Properties.Resources.icons8_help_48;
            pbString2Help.Location = new Point(214, 67);
            pbString2Help.Name = "pbString2Help";
            pbString2Help.Size = new Size(20, 20);
            pbString2Help.SizeMode = PictureBoxSizeMode.Zoom;
            pbString2Help.TabIndex = 13;
            pbString2Help.TabStop = false;
            toolTip.SetToolTip(pbString2Help, "Replace type string2 with string. Only useful when working with old table format");
            // 
            // btnStart
            // 
            btnStart.Location = new Point(286, 28);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(75, 58);
            btnStart.TabIndex = 8;
            btnStart.Text = "Start";
            toolTip.SetToolTip(btnStart, "Process the files on the selected folder");
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += BtnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(375, 28);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(75, 58);
            btnStop.TabIndex = 6;
            btnStop.Text = "Stop";
            toolTip.SetToolTip(btnStop, "Cancels the current conversion operation");
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += BtnStop_Click;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(464, 28);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(75, 58);
            btnClear.TabIndex = 5;
            btnClear.Text = "Clear";
            toolTip.SetToolTip(btnClear, "Clear the output textbox");
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += BtnClear_Click;
            // 
            // cbxRepString2
            // 
            cbxRepString2.AutoSize = true;
            cbxRepString2.Location = new Point(108, 69);
            cbxRepString2.Name = "cbxRepString2";
            cbxRepString2.Size = new Size(107, 19);
            cbxRepString2.TabIndex = 4;
            cbxRepString2.Text = "Replace String2";
            cbxRepString2.UseVisualStyleBackColor = true;
            // 
            // cbxType
            // 
            cbxType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxType.FormattingEnabled = true;
            cbxType.Items.AddRange(new object[] { "XLSX", "XML", "XMLV2" });
            cbxType.Location = new Point(164, 28);
            cbxType.Name = "cbxType";
            cbxType.Size = new Size(68, 23);
            cbxType.TabIndex = 3;
            // 
            // lbFileType
            // 
            lbFileType.AutoSize = true;
            lbFileType.Location = new Point(104, 31);
            lbFileType.Name = "lbFileType";
            lbFileType.Size = new Size(55, 15);
            lbFileType.TabIndex = 2;
            lbFileType.Text = "File Type:";
            // 
            // rbtnDecrypt
            // 
            rbtnDecrypt.AutoSize = true;
            rbtnDecrypt.Checked = true;
            rbtnDecrypt.Location = new Point(6, 68);
            rbtnDecrypt.Name = "rbtnDecrypt";
            rbtnDecrypt.Size = new Size(83, 19);
            rbtnDecrypt.TabIndex = 1;
            rbtnDecrypt.TabStop = true;
            rbtnDecrypt.Text = "Decryption";
            rbtnDecrypt.UseVisualStyleBackColor = true;
            // 
            // rbtnEncrypt
            // 
            rbtnEncrypt.AutoSize = true;
            rbtnEncrypt.Location = new Point(6, 29);
            rbtnEncrypt.Name = "rbtnEncrypt";
            rbtnEncrypt.Size = new Size(82, 19);
            rbtnEncrypt.TabIndex = 0;
            rbtnEncrypt.Text = "Encryption";
            rbtnEncrypt.UseVisualStyleBackColor = true;
            // 
            // lbVersion
            // 
            lbVersion.AutoSize = true;
            lbVersion.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            lbVersion.Location = new Point(486, 453);
            lbVersion.Name = "lbVersion";
            lbVersion.Size = new Size(57, 19);
            lbVersion.TabIndex = 6;
            lbVersion.Text = "Version:";
            toolTip.SetToolTip(lbVersion, "Click to open github repository.");
            lbVersion.Click += LbVersion_Click;
            // 
            // btnOpenInputFolder
            // 
            btnOpenInputFolder.Location = new Point(490, 12);
            btnOpenInputFolder.Name = "btnOpenInputFolder";
            btnOpenInputFolder.Size = new Size(69, 22);
            btnOpenInputFolder.TabIndex = 10;
            btnOpenInputFolder.Text = "Open Dir";
            toolTip.SetToolTip(btnOpenInputFolder, "Opens the Input directory on explorer");
            btnOpenInputFolder.UseVisualStyleBackColor = true;
            btnOpenInputFolder.Click += BtnOpenInputDir_Click;
            // 
            // btnOpenOutputFolder
            // 
            btnOpenOutputFolder.Location = new Point(490, 41);
            btnOpenOutputFolder.Name = "btnOpenOutputFolder";
            btnOpenOutputFolder.Size = new Size(69, 22);
            btnOpenOutputFolder.TabIndex = 11;
            btnOpenOutputFolder.Text = "Open Dir";
            toolTip.SetToolTip(btnOpenOutputFolder, "Opens the Output directory on explorer");
            btnOpenOutputFolder.UseVisualStyleBackColor = true;
            btnOpenOutputFolder.Click += BtnOpenOutputDir_Click;
            // 
            // pbHelp
            // 
            pbHelp.Cursor = Cursors.Help;
            pbHelp.Image = Properties.Resources.icons8_help_48;
            pbHelp.Location = new Point(14, 80);
            pbHelp.Name = "pbHelp";
            pbHelp.Size = new Size(20, 20);
            pbHelp.SizeMode = PictureBoxSizeMode.Zoom;
            pbHelp.TabIndex = 12;
            pbHelp.TabStop = false;
            toolTip.SetToolTip(pbHelp, "You can also Drag and drop files/folders into the text box bellow for processing");
            // 
            // btnInputFolder
            // 
            btnInputFolder.Location = new Point(411, 12);
            btnInputFolder.Name = "btnInputFolder";
            btnInputFolder.Size = new Size(69, 22);
            btnInputFolder.TabIndex = 3;
            btnInputFolder.Text = "Browse";
            btnInputFolder.UseVisualStyleBackColor = true;
            btnInputFolder.Click += BtnInputFolder_Click;
            // 
            // tbInputFolder
            // 
            tbInputFolder.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            tbInputFolder.Location = new Point(96, 13);
            tbInputFolder.Name = "tbInputFolder";
            tbInputFolder.Size = new Size(300, 22);
            tbInputFolder.TabIndex = 4;
            tbInputFolder.TextChanged += TbInputFolder_TextChanged;
            // 
            // lbInputFolder
            // 
            lbInputFolder.AutoSize = true;
            lbInputFolder.Location = new Point(12, 16);
            lbInputFolder.Name = "lbInputFolder";
            lbInputFolder.Size = new Size(71, 15);
            lbInputFolder.TabIndex = 5;
            lbInputFolder.Text = "Input Folder";
            // 
            // lbOutputFolder
            // 
            lbOutputFolder.AutoSize = true;
            lbOutputFolder.Location = new Point(12, 45);
            lbOutputFolder.Name = "lbOutputFolder";
            lbOutputFolder.Size = new Size(81, 15);
            lbOutputFolder.TabIndex = 9;
            lbOutputFolder.Text = "Output Folder";
            // 
            // tbOutputFolder
            // 
            tbOutputFolder.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            tbOutputFolder.Location = new Point(96, 41);
            tbOutputFolder.Name = "tbOutputFolder";
            tbOutputFolder.Size = new Size(300, 22);
            tbOutputFolder.TabIndex = 8;
            tbOutputFolder.TextChanged += TbOutputFolder_TextChanged;
            // 
            // btnOutputFolder
            // 
            btnOutputFolder.Location = new Point(411, 41);
            btnOutputFolder.Name = "btnOutputFolder";
            btnOutputFolder.Size = new Size(69, 22);
            btnOutputFolder.TabIndex = 7;
            btnOutputFolder.Text = "Browse";
            btnOutputFolder.UseVisualStyleBackColor = true;
            btnOutputFolder.Click += BtnOutputFolder_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(580, 474);
            Controls.Add(pbHelp);
            Controls.Add(btnOpenOutputFolder);
            Controls.Add(btnOpenInputFolder);
            Controls.Add(lbOutputFolder);
            Controls.Add(tbOutputFolder);
            Controls.Add(btnOutputFolder);
            Controls.Add(lbVersion);
            Controls.Add(lbInputFolder);
            Controls.Add(tbInputFolder);
            Controls.Add(btnInputFolder);
            Controls.Add(gbCryptor);
            Controls.Add(txtFile);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Rusty Hearts Table Tool";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            gbCryptor.ResumeLayout(false);
            gbCryptor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbString2Help).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbHelp).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtFile;
        private GroupBox gbCryptor;
        private Button btnStop;
        private Button btnClear;
        private CheckBox cbxRepString2;
        private ComboBox cbxType;
        private Label lbFileType;
        private RadioButton rbtnDecrypt;
        private RadioButton rbtnEncrypt;
        private ToolTip toolTip;
        private Button btnInputFolder;
        private TextBox tbInputFolder;
        private Label lbInputFolder;
        private Button btnStart;
        private Label lbVersion;
        private Label lbOutputFolder;
        private TextBox tbOutputFolder;
        private Button btnOutputFolder;
        private Button btnOpenInputFolder;
        private Button btnOpenOutputFolder;
        private PictureBox pbHelp;
        private PictureBox pbString2Help;
    }
}