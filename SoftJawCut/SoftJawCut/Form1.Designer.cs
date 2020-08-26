namespace SoftJawCut
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbRough = new System.Windows.Forms.ComboBox();
            this.cbFinish = new System.Windows.Forms.ComboBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.nudDeep = new System.Windows.Forms.NumericUpDown();
            this.nudUp = new System.Windows.Forms.NumericUpDown();
            this.nudDown = new System.Windows.Forms.NumericUpDown();
            this.cbSameSize = new System.Windows.Forms.CheckBox();
            this.txtOutFile = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnChooseFile = new System.Windows.Forms.Button();
            this.saveDiag = new System.Windows.Forms.SaveFileDialog();
            this.nudCutLen = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudDeep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCutLen)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Rough Dia:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 34);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Finish Dia:";
            // 
            // cbRough
            // 
            this.cbRough.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRough.FormattingEnabled = true;
            this.cbRough.Items.AddRange(new object[] {
            "250",
            "375",
            "500"});
            this.cbRough.Location = new System.Drawing.Point(115, 6);
            this.cbRough.Name = "cbRough";
            this.cbRough.Size = new System.Drawing.Size(80, 21);
            this.cbRough.TabIndex = 2;
            // 
            // cbFinish
            // 
            this.cbFinish.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFinish.FormattingEnabled = true;
            this.cbFinish.Items.AddRange(new object[] {
            "250",
            "375",
            "500"});
            this.cbFinish.Location = new System.Drawing.Point(115, 31);
            this.cbFinish.Name = "cbFinish";
            this.cbFinish.Size = new System.Drawing.Size(80, 21);
            this.cbFinish.TabIndex = 3;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(323, 287);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnGenerate.TabIndex = 4;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 59);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Deep:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 84);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Up Side Length:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 109);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Down Side Length:";
            // 
            // nudDeep
            // 
            this.nudDeep.Location = new System.Drawing.Point(115, 57);
            this.nudDeep.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.nudDeep.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudDeep.Name = "nudDeep";
            this.nudDeep.Size = new System.Drawing.Size(80, 20);
            this.nudDeep.TabIndex = 11;
            this.nudDeep.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // nudUp
            // 
            this.nudUp.Location = new System.Drawing.Point(115, 82);
            this.nudUp.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudUp.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudUp.Name = "nudUp";
            this.nudUp.Size = new System.Drawing.Size(80, 20);
            this.nudUp.TabIndex = 12;
            this.nudUp.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudUp.ValueChanged += new System.EventHandler(this.nudUp_ValueChanged);
            // 
            // nudDown
            // 
            this.nudDown.Location = new System.Drawing.Point(115, 107);
            this.nudDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudDown.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudDown.Name = "nudDown";
            this.nudDown.Size = new System.Drawing.Size(80, 20);
            this.nudDown.TabIndex = 13;
            this.nudDown.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // cbSameSize
            // 
            this.cbSameSize.AutoSize = true;
            this.cbSameSize.Location = new System.Drawing.Point(15, 135);
            this.cbSameSize.Name = "cbSameSize";
            this.cbSameSize.Size = new System.Drawing.Size(113, 17);
            this.cbSameSize.TabIndex = 14;
            this.cbSameSize.Text = "Same Side Length";
            this.cbSameSize.UseVisualStyleBackColor = true;
            this.cbSameSize.CheckedChanged += new System.EventHandler(this.cbSameSize_CheckedChanged);
            // 
            // txtOutFile
            // 
            this.txtOutFile.Location = new System.Drawing.Point(15, 289);
            this.txtOutFile.Name = "txtOutFile";
            this.txtOutFile.Size = new System.Drawing.Size(272, 20);
            this.txtOutFile.TabIndex = 15;
            this.txtOutFile.Text = "C:\\Users\\Hoa Ho\\Desktop\\SOFTJAW.txt";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 271);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Output Folder:";
            // 
            // btnChooseFile
            // 
            this.btnChooseFile.Location = new System.Drawing.Point(293, 287);
            this.btnChooseFile.Name = "btnChooseFile";
            this.btnChooseFile.Size = new System.Drawing.Size(24, 23);
            this.btnChooseFile.TabIndex = 17;
            this.btnChooseFile.Text = "...";
            this.btnChooseFile.UseVisualStyleBackColor = true;
            this.btnChooseFile.Click += new System.EventHandler(this.btnChooseFile_Click);
            // 
            // nudCutLen
            // 
            this.nudCutLen.Location = new System.Drawing.Point(115, 165);
            this.nudCutLen.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.nudCutLen.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudCutLen.Name = "nudCutLen";
            this.nudCutLen.Size = new System.Drawing.Size(80, 20);
            this.nudCutLen.TabIndex = 19;
            this.nudCutLen.Value = new decimal(new int[] {
            6000,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 167);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Cut Length:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 319);
            this.Controls.Add(this.nudCutLen);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnChooseFile);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtOutFile);
            this.Controls.Add(this.cbSameSize);
            this.Controls.Add(this.nudDown);
            this.Controls.Add(this.nudUp);
            this.Controls.Add(this.nudDeep);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.cbFinish);
            this.Controls.Add(this.cbRough);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "SoftJaw Cut Generator";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudDeep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCutLen)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbRough;
        private System.Windows.Forms.ComboBox cbFinish;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudDeep;
        private System.Windows.Forms.NumericUpDown nudUp;
        private System.Windows.Forms.NumericUpDown nudDown;
        private System.Windows.Forms.CheckBox cbSameSize;
        private System.Windows.Forms.TextBox txtOutFile;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnChooseFile;
        private System.Windows.Forms.SaveFileDialog saveDiag;
        private System.Windows.Forms.NumericUpDown nudCutLen;
        private System.Windows.Forms.Label label7;
    }
}

