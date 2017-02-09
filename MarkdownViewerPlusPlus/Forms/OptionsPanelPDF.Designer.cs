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
            this.cmbPDFOrientation = new System.Windows.Forms.ComboBox();
            this.lblPDFOrientation = new System.Windows.Forms.Label();
            this.grpPDFExport.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPDFExport
            // 
            this.grpPDFExport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpPDFExport.Controls.Add(this.cmbPDFOrientation);
            this.grpPDFExport.Controls.Add(this.lblPDFOrientation);
            this.grpPDFExport.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpPDFExport.Location = new System.Drawing.Point(0, 0);
            this.grpPDFExport.Name = "grpPDFExport";
            this.grpPDFExport.Size = new System.Drawing.Size(1255, 61);
            this.grpPDFExport.TabIndex = 0;
            this.grpPDFExport.TabStop = false;
            this.grpPDFExport.Text = "Export";
            // 
            // cmbPDFOrientation
            // 
            this.cmbPDFOrientation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPDFOrientation.FormattingEnabled = true;
            this.cmbPDFOrientation.Location = new System.Drawing.Point(10, 36);
            this.cmbPDFOrientation.Name = "cmbPDFOrientation";
            this.cmbPDFOrientation.Size = new System.Drawing.Size(274, 21);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpPDFExport;
        private System.Windows.Forms.Label lblPDFOrientation;
        private System.Windows.Forms.ComboBox cmbPDFOrientation;
    }
}
