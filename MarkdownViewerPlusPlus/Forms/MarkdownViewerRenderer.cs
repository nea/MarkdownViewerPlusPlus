using Kbg.NppPluginNET.PluginInfrastructure;
using PdfSharp;
using PdfSharp.Pdf;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using TheArtOfDev.HtmlRenderer.WinForms;

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
        protected HtmlPanel markdownViewerHtmlPanel;

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
            this.markdownViewerHtmlPanel = new HtmlPanel();
            this.markdownViewerHtmlPanel.AllowDrop = false;
            this.markdownViewerHtmlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.markdownViewerHtmlPanel.IsContextMenuEnabled = false;
            this.markdownViewerHtmlPanel.Location = new System.Drawing.Point(0, 24);
            this.markdownViewerHtmlPanel.MinimumSize = new System.Drawing.Size(20, 20);
            this.markdownViewerHtmlPanel.Name = "markdownViewerHtmlPanel";
            this.markdownViewerHtmlPanel.Size = new System.Drawing.Size(284, 237);
            this.markdownViewerHtmlPanel.TabIndex = 0;
            //
            this.Controls.Add(this.markdownViewerHtmlPanel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        public override void Render(string html)
        {
            this.markdownViewerHtmlPanel.Text = BuildHtml(html);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetText()
        {
            return this.markdownViewerHtmlPanel.Text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void exportAsHTMLMenuItem_Click(object sender, EventArgs e)
        {
            //The current file name
            StringBuilder fileName = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETFILENAME, 0, fileName);
            //The current path
            StringBuilder path = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETCURRENTDIRECTORY, 0, path);

            //Save!
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //Default name of the file is the editor file name
            saveFileDialog.FileName = fileName.ToString() + ".html";
            saveFileDialog.InitialDirectory = path.ToString();
            //
            saveFileDialog.Filter = "HTML files (*.html)|*.html|All files (*.*)|*.*";
            //
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName))
                {
                    sw.WriteLine(BuildHtml(GetText(), fileName.ToString()));
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
            StringBuilder filename = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETFILENAME, 0, filename);
            //The current path
            StringBuilder path = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETCURRENTDIRECTORY, 0, path);

            //Save!
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //Default name of the file is the editor file name
            saveFileDialog.FileName = filename.ToString() + ".pdf";
            saveFileDialog.InitialDirectory = path.ToString();
            //
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            //
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                PdfDocument pdf = PdfGenerator.GeneratePdf(BuildHtml(GetText(), filename.ToString()), PageSize.A4);
                pdf.Save(saveFileDialog.FileName);
            }
        }        
    }
}
