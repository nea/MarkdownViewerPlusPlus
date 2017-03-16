using TheArtOfDev.HtmlRenderer.WinForms;

/// <summary>
/// 
/// </summary>
namespace com.insanitydesign.MarkdownViewerPlusPlus.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public class MarkdownViewerHtmlPanel : HtmlPanel
    {
        /// <summary>
        /// 
        /// </summary>
        public MarkdownViewerHtmlPanel()
        {
            this.AllowDrop = false;
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IsContextMenuEnabled = false;
            this.Location = new System.Drawing.Point(0, 24);
            this.MinimumSize = new System.Drawing.Size(20, 20);
            this.Name = "markdownViewerHtmlPanel";
            this.Size = new System.Drawing.Size(284, 237);
            this.TabIndex = 0;
            this.AvoidImagesLateLoading = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Text {
            get { return _text; }
            set {
                _text = value;
                if (!IsDisposed)
                {
                    _htmlContainer.SetHtml(_text, _baseCssData);
                    Redraw();
                }
            }
        }

        /// <summary>
        /// Scroll by the given ratio, calculated with max and page
        /// </summary>
        /// <param name="scrollRatio"></param>
        public void ScrollByRatioVertically(double scrollRatio)
        {
            if (!IsDisposed)
            {
                VerticalScroll.Value = (int)((VerticalScroll.Maximum - VerticalScroll.LargeChange) * scrollRatio);
                Redraw();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Redraw()
        {
            PerformLayout();
            Invalidate();
            InvokeMouseMove();
        }
    }
}
