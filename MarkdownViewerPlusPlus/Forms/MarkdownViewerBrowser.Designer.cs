namespace com.insanitydesign.MarkdownViewerPlusPlus.Forms
{
    partial class MarkdownViewerBrowser
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.markdownViewerWebBrowser = new System.Windows.Forms.WebBrowser();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.exportAsToolStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAsHTMLMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAsPDFMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // markdownViewerWebBrowser
            // 
            this.markdownViewerWebBrowser.AllowWebBrowserDrop = false;
            this.markdownViewerWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.markdownViewerWebBrowser.IsWebBrowserContextMenuEnabled = false;
            this.markdownViewerWebBrowser.Location = new System.Drawing.Point(0, 24);
            this.markdownViewerWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.markdownViewerWebBrowser.Name = "markdownViewerWebBrowser";
            this.markdownViewerWebBrowser.ScriptErrorsSuppressed = true;
            this.markdownViewerWebBrowser.Size = new System.Drawing.Size(284, 237);
            this.markdownViewerWebBrowser.TabIndex = 0;
            this.markdownViewerWebBrowser.WebBrowserShortcutsEnabled = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportAsToolStrip,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(284, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
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
            this.Controls.Add(this.markdownViewerWebBrowser);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MarkdownViewerBrowser";
            this.Text = "MarkdownViewerBrowser";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser markdownViewerWebBrowser;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exportAsToolStrip;
        private System.Windows.Forms.ToolStripMenuItem exportAsHTMLMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAsPDFMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
    }
}