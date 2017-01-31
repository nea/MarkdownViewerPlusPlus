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
        public override string Text {
            get { return _text; }
            set {
                _text = value;
                //base.Text = value;
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
