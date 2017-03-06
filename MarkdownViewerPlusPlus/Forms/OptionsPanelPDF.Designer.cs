namespace com.insanitydesign.MarkdownViewerPlusPlus.Forms
{
    partial class OptionsPanelPDF
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        protected override void InitializeComponent()
        {
            this.grpPDFExport = new System.Windows.Forms.GroupBox();
            this.numMarginLeft = new System.Windows.Forms.NumericUpDown();
            this.numMarginRight = new System.Windows.Forms.NumericUpDown();
            this.numMarginBottom = new System.Windows.Forms.NumericUpDown();
            this.numMarginTop = new System.Windows.Forms.NumericUpDown();
            this.lblMMBottom = new System.Windows.Forms.Label();
            this.lblMMRight = new System.Windows.Forms.Label();
            this.lblMMLeft = new System.Windows.Forms.Label();
            this.lblMMTop = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbPDFPageSize = new System.Windows.Forms.ComboBox();
            this.lblPDFPageSize = new System.Windows.Forms.Label();
            this.cmbPDFOrientation = new System.Windows.Forms.ComboBox();
            this.lblPDFOrientation = new System.Windows.Forms.Label();
            this.chkOpenPDFExport = new System.Windows.Forms.CheckBox();
            this.grpPDFExport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginTop)).BeginInit();
            this.SuspendLayout();
            // 
            // grpPDFExport
            // 
            this.grpPDFExport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpPDFExport.Controls.Add(this.chkOpenPDFExport);
            this.grpPDFExport.Controls.Add(this.numMarginLeft);
            this.grpPDFExport.Controls.Add(this.numMarginRight);
            this.grpPDFExport.Controls.Add(this.numMarginBottom);
            this.grpPDFExport.Controls.Add(this.numMarginTop);
            this.grpPDFExport.Controls.Add(this.lblMMBottom);
            this.grpPDFExport.Controls.Add(this.lblMMRight);
            this.grpPDFExport.Controls.Add(this.lblMMLeft);
            this.grpPDFExport.Controls.Add(this.lblMMTop);
            this.grpPDFExport.Controls.Add(this.label1);
            this.grpPDFExport.Controls.Add(this.cmbPDFPageSize);
            this.grpPDFExport.Controls.Add(this.lblPDFPageSize);
            this.grpPDFExport.Controls.Add(this.cmbPDFOrientation);
            this.grpPDFExport.Controls.Add(this.lblPDFOrientation);
            this.grpPDFExport.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpPDFExport.Location = new System.Drawing.Point(0, 0);
            this.grpPDFExport.Name = "grpPDFExport";
            this.grpPDFExport.Size = new System.Drawing.Size(1255, 235);
            this.grpPDFExport.TabIndex = 0;
            this.grpPDFExport.TabStop = false;
            this.grpPDFExport.Text = "Export";
            // 
            // numMarginLeft
            // 
            this.numMarginLeft.Location = new System.Drawing.Point(10, 150);
            this.numMarginLeft.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numMarginLeft.Name = "numMarginLeft";
            this.numMarginLeft.Size = new System.Drawing.Size(51, 20);
            this.numMarginLeft.TabIndex = 19;
            // 
            // numMarginRight
            // 
            this.numMarginRight.Location = new System.Drawing.Point(190, 150);
            this.numMarginRight.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numMarginRight.Name = "numMarginRight";
            this.numMarginRight.Size = new System.Drawing.Size(50, 20);
            this.numMarginRight.TabIndex = 18;
            // 
            // numMarginBottom
            // 
            this.numMarginBottom.Location = new System.Drawing.Point(100, 175);
            this.numMarginBottom.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numMarginBottom.Name = "numMarginBottom";
            this.numMarginBottom.Size = new System.Drawing.Size(50, 20);
            this.numMarginBottom.TabIndex = 17;
            // 
            // numMarginTop
            // 
            this.numMarginTop.Location = new System.Drawing.Point(100, 125);
            this.numMarginTop.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numMarginTop.Name = "numMarginTop";
            this.numMarginTop.Size = new System.Drawing.Size(50, 20);
            this.numMarginTop.TabIndex = 16;
            // 
            // lblMMBottom
            // 
            this.lblMMBottom.AutoSize = true;
            this.lblMMBottom.Location = new System.Drawing.Point(157, 181);
            this.lblMMBottom.Name = "lblMMBottom";
            this.lblMMBottom.Size = new System.Drawing.Size(23, 13);
            this.lblMMBottom.TabIndex = 15;
            this.lblMMBottom.Text = "mm";
            // 
            // lblMMRight
            // 
            this.lblMMRight.AutoSize = true;
            this.lblMMRight.Location = new System.Drawing.Point(247, 156);
            this.lblMMRight.Name = "lblMMRight";
            this.lblMMRight.Size = new System.Drawing.Size(23, 13);
            this.lblMMRight.TabIndex = 14;
            this.lblMMRight.Text = "mm";
            // 
            // lblMMLeft
            // 
            this.lblMMLeft.AutoSize = true;
            this.lblMMLeft.Location = new System.Drawing.Point(66, 157);
            this.lblMMLeft.Name = "lblMMLeft";
            this.lblMMLeft.Size = new System.Drawing.Size(23, 13);
            this.lblMMLeft.TabIndex = 13;
            this.lblMMLeft.Text = "mm";
            // 
            // lblMMTop
            // 
            this.lblMMTop.AutoSize = true;
            this.lblMMTop.Location = new System.Drawing.Point(156, 132);
            this.lblMMTop.Name = "lblMMTop";
            this.lblMMTop.Size = new System.Drawing.Size(23, 13);
            this.lblMMTop.TabIndex = 12;
            this.lblMMTop.Text = "mm";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Margins";
            // 
            // cmbPDFPageSize
            // 
            this.cmbPDFPageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPDFPageSize.FormattingEnabled = true;
            this.cmbPDFPageSize.Location = new System.Drawing.Point(10, 81);
            this.cmbPDFPageSize.Name = "cmbPDFPageSize";
            this.cmbPDFPageSize.Size = new System.Drawing.Size(300, 21);
            this.cmbPDFPageSize.TabIndex = 6;
            // 
            // lblPDFPageSize
            // 
            this.lblPDFPageSize.AutoSize = true;
            this.lblPDFPageSize.Location = new System.Drawing.Point(7, 65);
            this.lblPDFPageSize.Name = "lblPDFPageSize";
            this.lblPDFPageSize.Size = new System.Drawing.Size(53, 13);
            this.lblPDFPageSize.TabIndex = 5;
            this.lblPDFPageSize.Text = "Page size";
            // 
            // cmbPDFOrientation
            // 
            this.cmbPDFOrientation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPDFOrientation.FormattingEnabled = true;
            this.cmbPDFOrientation.Location = new System.Drawing.Point(10, 36);
            this.cmbPDFOrientation.Name = "cmbPDFOrientation";
            this.cmbPDFOrientation.Size = new System.Drawing.Size(300, 21);
            this.cmbPDFOrientation.TabIndex = 4;
            // 
            // lblPDFOrientation
            // 
            this.lblPDFOrientation.AutoSize = true;
            this.lblPDFOrientation.Location = new System.Drawing.Point(7, 20);
            this.lblPDFOrientation.Name = "lblPDFOrientation";
            this.lblPDFOrientation.Size = new System.Drawing.Size(58, 13);
            this.lblPDFOrientation.TabIndex = 0;
            this.lblPDFOrientation.Text = "Orientation";
            // 
            // chkOpenPDFExport
            // 
            this.chkOpenPDFExport.AutoSize = true;
            this.chkOpenPDFExport.Location = new System.Drawing.Point(10, 206);
            this.chkOpenPDFExport.Name = "chkOpenPDFExport";
            this.chkOpenPDFExport.Size = new System.Drawing.Size(132, 17);
            this.chkOpenPDFExport.TabIndex = 20;
            this.chkOpenPDFExport.Text = "Open PDF after export";
            this.chkOpenPDFExport.UseVisualStyleBackColor = true;
            // 
            // OptionsPanelPDF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.grpPDFExport);
            this.Name = "OptionsPanelPDF";
            this.Size = new System.Drawing.Size(1255, 709);
            this.grpPDFExport.ResumeLayout(false);
            this.grpPDFExport.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginTop)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpPDFExport;
        private System.Windows.Forms.Label lblPDFOrientation;
        private System.Windows.Forms.ComboBox cmbPDFOrientation;
        private System.Windows.Forms.ComboBox cmbPDFPageSize;
        private System.Windows.Forms.Label lblPDFPageSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblMMBottom;
        private System.Windows.Forms.Label lblMMRight;
        private System.Windows.Forms.Label lblMMLeft;
        private System.Windows.Forms.Label lblMMTop;
        private System.Windows.Forms.NumericUpDown numMarginLeft;
        private System.Windows.Forms.NumericUpDown numMarginRight;
        private System.Windows.Forms.NumericUpDown numMarginBottom;
        private System.Windows.Forms.NumericUpDown numMarginTop;
        private System.Windows.Forms.CheckBox chkOpenPDFExport;
    }
}
