namespace RHTableTool
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            txtFile = new TextBox();
            groupBox1 = new GroupBox();
            btnStart = new Button();
            btnOpenDir = new Button();
            btnStop = new Button();
            btnClear = new Button();
            cbxRepString2 = new CheckBox();
            cbxType = new ComboBox();
            lbFileType = new Label();
            rbtnDe = new RadioButton();
            rbtnEn = new RadioButton();
            label2 = new Label();
            toolTip1 = new ToolTip(components);
            btnBrowseFolder = new Button();
            tbBrowseFolder = new TextBox();
            lbFolder = new Label();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // txtFile
            // 
            txtFile.AllowDrop = true;
            txtFile.Location = new Point(12, 69);
            txtFile.Multiline = true;
            txtFile.Name = "txtFile";
            txtFile.ReadOnly = true;
            txtFile.ScrollBars = ScrollBars.Both;
            txtFile.Size = new Size(579, 201);
            txtFile.TabIndex = 0;
            txtFile.WordWrap = false;
            txtFile.DragDrop += txtFile_DragDrop;
            txtFile.DragEnter += txtFile_DragEnter;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnStart);
            groupBox1.Controls.Add(btnOpenDir);
            groupBox1.Controls.Add(btnStop);
            groupBox1.Controls.Add(btnClear);
            groupBox1.Controls.Add(cbxRepString2);
            groupBox1.Controls.Add(cbxType);
            groupBox1.Controls.Add(lbFileType);
            groupBox1.Controls.Add(rbtnDe);
            groupBox1.Controls.Add(rbtnEn);
            groupBox1.Location = new Point(12, 276);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(579, 108);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Decryption";
            // 
            // btnStart
            // 
            btnStart.Location = new Point(245, 28);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(75, 58);
            btnStart.TabIndex = 8;
            btnStart.Text = "Start";
            toolTip1.SetToolTip(btnStart, "Process the selected files on the selected path");
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnOpenDir
            // 
            btnOpenDir.Location = new Point(494, 28);
            btnOpenDir.Name = "btnOpenDir";
            btnOpenDir.Size = new Size(75, 58);
            btnOpenDir.TabIndex = 7;
            btnOpenDir.Text = "Output Directory";
            btnOpenDir.UseVisualStyleBackColor = true;
            btnOpenDir.Click += btnOpenDir_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(327, 28);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(75, 58);
            btnStop.TabIndex = 6;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(410, 28);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(75, 58);
            btnClear.TabIndex = 5;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // cbxRepString2
            // 
            cbxRepString2.AutoSize = true;
            cbxRepString2.Location = new Point(108, 69);
            cbxRepString2.Name = "cbxRepString2";
            cbxRepString2.Size = new Size(107, 19);
            cbxRepString2.TabIndex = 4;
            cbxRepString2.Text = "Replace String2";
            toolTip1.SetToolTip(cbxRepString2, "Replace type string2 with string, oly useful when working with old tables");
            cbxRepString2.UseVisualStyleBackColor = true;
            // 
            // cbxType
            // 
            cbxType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxType.FormattingEnabled = true;
            cbxType.Items.AddRange(new object[] { "XLSX", "XML" });
            cbxType.Location = new Point(164, 28);
            cbxType.Name = "cbxType";
            cbxType.Size = new Size(54, 23);
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
            // rbtnDe
            // 
            rbtnDe.AutoSize = true;
            rbtnDe.Checked = true;
            rbtnDe.Location = new Point(6, 68);
            rbtnDe.Name = "rbtnDe";
            rbtnDe.Size = new Size(83, 19);
            rbtnDe.TabIndex = 1;
            rbtnDe.TabStop = true;
            rbtnDe.Text = "Decryption";
            rbtnDe.UseVisualStyleBackColor = true;
            // 
            // rbtnEn
            // 
            rbtnEn.AutoSize = true;
            rbtnEn.Location = new Point(6, 29);
            rbtnEn.Name = "rbtnEn";
            rbtnEn.Size = new Size(82, 19);
            rbtnEn.TabIndex = 0;
            rbtnEn.Text = "Encryption";
            rbtnEn.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 51);
            label2.Name = "label2";
            label2.Size = new Size(194, 15);
            label2.TabIndex = 2;
            label2.Text = "Drag files/folders in the box bellow:";
            // 
            // btnBrowseFolder
            // 
            btnBrowseFolder.Location = new Point(443, 12);
            btnBrowseFolder.Name = "btnBrowseFolder";
            btnBrowseFolder.Size = new Size(69, 23);
            btnBrowseFolder.TabIndex = 3;
            btnBrowseFolder.Text = "Browse";
            btnBrowseFolder.UseVisualStyleBackColor = true;
            btnBrowseFolder.Click += btnBrowseFolder_Click;
            // 
            // tbBrowseFolder
            // 
            tbBrowseFolder.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            tbBrowseFolder.Location = new Point(92, 12);
            tbBrowseFolder.Name = "tbBrowseFolder";
            tbBrowseFolder.Size = new Size(345, 22);
            tbBrowseFolder.TabIndex = 4;
            // 
            // lbFolder
            // 
            lbFolder.AutoSize = true;
            lbFolder.Location = new Point(12, 15);
            lbFolder.Name = "lbFolder";
            lbFolder.Size = new Size(74, 15);
            lbFolder.TabIndex = 5;
            lbFolder.Text = "Select Folder";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(599, 387);
            Controls.Add(lbFolder);
            Controls.Add(tbBrowseFolder);
            Controls.Add(btnBrowseFolder);
            Controls.Add(label2);
            Controls.Add(groupBox1);
            Controls.Add(txtFile);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Rusty Hearts Table Tool";
            FormClosing += Form1_FormClosing;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtFile;
        private GroupBox groupBox1;
        private Button btnOpenDir;
        private Button btnStop;
        private Button btnClear;
        private CheckBox cbxRepString2;
        private ComboBox cbxType;
        private Label lbFileType;
        private RadioButton rbtnDe;
        private RadioButton rbtnEn;
        private Label label2;
        private ToolTip toolTip1;
        private Button btnBrowseFolder;
        private TextBox tbBrowseFolder;
        private Label lbFolder;
        private Button btnStart;
    }
}