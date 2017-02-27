using com.insanitydesign.MarkdownViewerPlusPlus.Properties;
using PdfSharp.Pdf;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using TheArtOfDev.HtmlRenderer.PdfSharp;

/// <summary>
/// 
/// </summary>
namespace com.insanitydesign.MarkdownViewerPlusPlus.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public class MarkdownViewerRenderer : AbstractRenderer
    {
        /// <summary>
        /// 
        /// </summary>
        public MarkdownViewerHtmlPanel markdownViewerHtmlPanel;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="markdownViewer"></param>
        public MarkdownViewerRenderer(MarkdownViewer markdownViewer) : base(markdownViewer)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Init()
        {
            base.Init();
            //
            this.markdownViewerHtmlPanel = new MarkdownViewerHtmlPanel();
            //Add to view
            this.Controls.Add(this.markdownViewerHtmlPanel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fileName"></param>
        public override void Render(string text, string fileName)
        {
            base.Render(text, fileName);
            this.markdownViewerHtmlPanel.Text = BuildHtml(ConvertedText, fileName);
        }

        /// <summary>
        /// Scroll the rendered panel vertically based on the given ration
        /// taken from Notepad++
        /// </summary>
        /// <param name="scrollRatio"></param>
        public override void ScrollByRatioVertically(double scrollRatio)
        {
            this.markdownViewerHtmlPanel.ScrollByRatioVertically(scrollRatio);
        }
    }
}
