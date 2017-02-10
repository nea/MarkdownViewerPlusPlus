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
        public override void Render(string text, string fileName = "")
        {
            base.Render(text, fileName);
            this.markdownViewerHtmlPanel.Text = BuildHtml(text, fileName);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void exportAsHTMLMenuItem_Click(object sender, EventArgs e)
        {
            //The current file name
            string fileName = this.markdownViewer.Notepad.GetCurrentFileName();
            //Save!
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //Default name of the file is the editor file name
            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(fileName) + ".html";
            //The current path
            saveFileDialog.InitialDirectory = this.markdownViewer.Notepad.GetCurrentDirectory();
            //
            saveFileDialog.Filter = "HTML files (*.html)|*.html|All files (*.*)|*.*";
            //
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName))
                {
                    string html = BuildHtml(GetText(), fileName);
                    try
                    {
                        html = XDocument.Parse(html).ToString();
                    }
                    catch { }
                    sw.WriteLine(html);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void exportAsPDFMenuItem_Click(object sender, EventArgs e)
        {
            //The current file name
            string fileName = this.markdownViewer.Notepad.GetCurrentFileName();
            //Save!
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //Default name of the file is the editor file name
            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(fileName) + ".pdf";
            //The current path
            saveFileDialog.InitialDirectory = this.markdownViewer.Notepad.GetCurrentDirectory();
            //
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            //
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Build a config based on made settings
                PdfGenerateConfig pdfConfig = new PdfGenerateConfig();
                pdfConfig.PageOrientation = this.markdownViewer.Options.pdfOrientation;
                pdfConfig.PageSize = this.markdownViewer.Options.pdfPageSize;
                //Generate PDF and save
                PdfDocument pdf = PdfGenerator.GeneratePdf(BuildHtml(GetText(), fileName), pdfConfig, PdfGenerator.ParseStyleSheet(Resources.MarkdownViewerHTML));                
                pdf.Save(saveFileDialog.FileName);
            }
        }
    }
}
