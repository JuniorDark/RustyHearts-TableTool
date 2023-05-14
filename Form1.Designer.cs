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
            btnOpenDir = new Button();
            btnStop = new Button();
            btnClear = new Button();
            cbxRepString2 = new CheckBox();
            cbxType = new ComboBox();
            label1 = new Label();
            rbtnDe = new RadioButton();
            rbtnEn = new RadioButton();
            label2 = new Label();
            toolTip1 = new ToolTip(components);
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // txtFile
            // 
            txtFile.AllowDrop = true;
            txtFile.Location = new Point(12, 38);
            txtFile.Multiline = true;
            txtFile.Name = "txtFile";
            txtFile.ReadOnly = true;
            txtFile.ScrollBars = ScrollBars.Both;
            txtFile.Size = new Size(500, 223);
            txtFile.TabIndex = 0;
            txtFile.WordWrap = false;
            txtFile.DragDrop += txtFile_DragDrop;
            txtFile.DragEnter += txtFile_DragEnter;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnOpenDir);
            groupBox1.Controls.Add(btnStop);
            groupBox1.Controls.Add(btnClear);
            groupBox1.Controls.Add(cbxRepString2);
            groupBox1.Controls.Add(cbxType);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(rbtnDe);
            groupBox1.Controls.Add(rbtnEn);
            groupBox1.Location = new Point(12, 276);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(506, 108);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Decryption";
            // 
            // btnOpenDir
            // 
            btnOpenDir.Location = new Point(416, 28);
            btnOpenDir.Name = "btnOpenDir";
            btnOpenDir.Size = new Size(75, 58);
            btnOpenDir.TabIndex = 7;
            btnOpenDir.Text = "Output Directory";
            btnOpenDir.UseVisualStyleBackColor = true;
            btnOpenDir.Click += btnOpenDir_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(326, 28);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(75, 58);
            btnStop.TabIndex = 6;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(233, 28);
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
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(104, 31);
            label1.Name = "label1";
            label1.Size = new Size(55, 15);
            label1.TabIndex = 2;
            label1.Text = "File Type:";
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
            label2.Location = new Point(12, 13);
            label2.Name = "label2";
            label2.Size = new Size(194, 15);
            label2.TabIndex = 2;
            label2.Text = "Drag files/folders in the box bellow:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(520, 387);
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
        private Label label1;
        private RadioButton rbtnDe;
        private RadioButton rbtnEn;
        private Label label2;
        private ToolTip toolTip1;
    }
}