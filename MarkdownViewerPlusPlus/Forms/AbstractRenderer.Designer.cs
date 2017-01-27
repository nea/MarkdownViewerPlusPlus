/// <summary>
/// 
/// </summary>
namespace com.insanitydesign.MarkdownViewerPlusPlus.Forms
{
    /// <summary>
    /// 
    /// </summary>
    partial class AbstractRenderer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        protected System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        protected void InitializeComponent()
        {            
            this.markdownViewerMenuStrip = new System.Windows.Forms.MenuStrip();
            this.exportAsToolStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAsHTMLMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAsPDFMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.markdownViewerMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // markdownViewerMenuStrip
            // 
            this.markdownViewerMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportAsToolStrip,
            this.viewToolStripMenuItem});
            this.markdownViewerMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.markdownViewerMenuStrip.Name = "markdownViewerMenuStrip";
            this.markdownViewerMenuStrip.Size = new System.Drawing.Size(284, 24);
            this.markdownViewerMenuStrip.TabIndex = 1;
            this.markdownViewerMenuStrip.Text = "markdownViewerMenuStrip";
            // 
            // exportAsToolStrip
            // 
            this.exportAsToolStrip.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportAsHTMLMenuItem,
            this.exportAsPDFMenuItem});
            this.exportAsToolStrip.Image = global::com.insanitydesign.MarkdownViewerPlusPlus.Properties.Resources.fa_download_16x16;
            this.exportAsToolStrip.Name = "exportAsToolStrip";
            this.exportAsToolStrip.Size = new System.Drawing.Size(82, 20);
            this.exportAsToolStrip.Text = "Export as";
            // 
            // exportAsHTMLMenuItem
            // 
            this.exportAsHTMLMenuItem.Image = global::com.insanitydesign.MarkdownViewerPlusPlus.Properties.Resources.fa_html5_16x16;
            this.exportAsHTMLMenuItem.Name = "exportAsHTMLMenuItem";
            this.exportAsHTMLMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exportAsHTMLMenuItem.Text = "HTML";
            this.exportAsHTMLMenuItem.Click += new System.EventHandler(this.exportAsHTMLMenuItem_Click);
            // 
            // exportAsPDFMenuItem
            // 
            this.exportAsPDFMenuItem.Image = global::com.insanitydesign.MarkdownViewerPlusPlus.Properties.Resources.fa_file_pdf_o_16x16;
            this.exportAsPDFMenuItem.Name = "exportAsPDFMenuItem";
            this.exportAsPDFMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exportAsPDFMenuItem.Text = "PDF";
            this.exportAsPDFMenuItem.Click += new System.EventHandler(this.exportAsPDFMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem});
            this.viewToolStripMenuItem.Image = global::com.insanitydesign.MarkdownViewerPlusPlus.Properties.Resources.fa_tv_16x16;
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Image = global::com.insanitydesign.MarkdownViewerPlusPlus.Properties.Resources.fa_refresh_16x16;
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // MarkdownViewerBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);            
            this.Controls.Add(this.markdownViewerMenuStrip);
            this.MainMenuStrip = this.markdownViewerMenuStrip;
            this.Name = "MarkdownViewer++";
            this.Text = "MarkdownViewer++";
            this.markdownViewerMenuStrip.ResumeLayout(false);
            this.markdownViewerMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        protected System.Windows.Forms.MenuStrip markdownViewerMenuStrip;
        protected System.Windows.Forms.ToolStripMenuItem exportAsToolStrip;
        protected System.Windows.Forms.ToolStripMenuItem exportAsHTMLMenuItem;
        protected System.Windows.Forms.ToolStripMenuItem exportAsPDFMenuItem;
        protected System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        protected System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
    }
}
