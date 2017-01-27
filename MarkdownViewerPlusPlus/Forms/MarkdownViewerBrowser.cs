/// <summary>
/// 
/// </summary>
namespace com.insanitydesign.MarkdownViewerPlusPlus.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public class MarkdownViewerBrowser : MarkdownViewerRenderer
    {
        /// <summary>
        /// 
        /// </summary>
        private System.Windows.Forms.WebBrowser markdownViewerWebBrowser;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="markdownViewer"></param>
        public MarkdownViewerBrowser(MarkdownViewer markdownViewer) : base(markdownViewer)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Init()
        {
            base.Init();
            //
            this.markdownViewerWebBrowser = new System.Windows.Forms.WebBrowser();
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
            this.Controls.Add(this.markdownViewerWebBrowser);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        public override void Render(string html)
        {
            this.markdownViewerWebBrowser.DocumentText = BuildHtml(html);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetText()
        {
            return this.markdownViewerWebBrowser.DocumentText;
        }
    }
}
