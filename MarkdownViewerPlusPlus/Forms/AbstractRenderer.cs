using com.insanitydesign.MarkdownViewerPlusPlus.Properties;
using com.insanitydesign.MarkdownViewerPlusPlus.Windows;
using Kbg.NppPluginNET;
using Kbg.NppPluginNET.PluginInfrastructure;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Outlook = Microsoft.Office.Interop.Outlook;
using static com.insanitydesign.MarkdownViewerPlusPlus.Windows.WindowsMessage;
using System.Drawing.Printing;
using CommonMark;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using System.Xml.Linq;
using PdfSharp.Pdf;
using System.IO;
using System.Net;
using com.insanitydesign.MarkdownViewerPlusPlus.Helper;
using static com.insanitydesign.MarkdownViewerPlusPlus.MarkdownViewer;

/// <summary>
/// 
/// </summary>
namespace com.insanitydesign.MarkdownViewerPlusPlus.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public abstract partial class AbstractRenderer : Form
    {
        /// <summary>
        /// 
        /// </summary>
        protected Icon toolbarIcon;

        /// <summary>
        /// 
        /// </summary>
        protected MarkdownViewer markdownViewer;

        /// <summary>
        /// 
        /// </summary>
        protected string assemblyTitle;

        /// <summary>
        /// 
        /// </summary>
        protected virtual string RawText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected virtual string ConvertedText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected virtual FileInformation FileInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="markdownViewer"></param>
        public AbstractRenderer(MarkdownViewer markdownViewer)
        {
            this.markdownViewer = markdownViewer;
            InitializeComponent();
            Init();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Init()
        {
            //
            using (Bitmap newBmp = new Bitmap(16, 16))
            {
                Graphics g = Graphics.FromImage(newBmp);
                ColorMap[] colorMap = new ColorMap[1];
                colorMap[0] = new ColorMap();
                colorMap[0].OldColor = Color.Fuchsia;
                colorMap[0].NewColor = Color.FromKnownColor(KnownColor.ButtonFace);
                ImageAttributes attr = new ImageAttributes();
                attr.SetRemapTable(colorMap);
                g.DrawImage(Resources.markdown_16x16_solid_bmp, new Rectangle(0, 0, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel, attr);
                toolbarIcon = Icon.FromHandle(newBmp.GetHicon());
            }
            //Get the AssemblyTitle
            this.assemblyTitle = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false)).Title;
            //
            NppTbData _nppTbData = new NppTbData();
            _nppTbData.hClient = this.Handle;
            _nppTbData.pszName = this.assemblyTitle;
            _nppTbData.dlgID = this.markdownViewer.commandId;
            _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
            _nppTbData.hIconTab = (uint)toolbarIcon.Handle;
            _nppTbData.pszModuleName = Main.PluginName;
            IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
            Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);
            //Register dockable window and hide initially
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMHIDE, 0, this.Handle);

            //Hide the E-mail items if Outlook is not installed
            this.sendAsTextMail.Visible = IsOutlookInstalled();
            this.sendAsHTMLMail.Visible = IsOutlookInstalled();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            //Listen for the closing of the dockable panel to toggle the toolbar icon
            switch (m.Msg)
            {
                case (int)WM_NOTIFY:
                    var notify = (NMHDR)Marshal.PtrToStructure(m.LParam, typeof(NMHDR));
                    if (notify.code == (int)DockMgrMsg.DMN_CLOSE)
                    {
                        this.markdownViewer.ToggleToolbarIcon(false);
                    }
                    break;
            }
            //Continue the processing, as we only toggle
            base.WndProc(ref m);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fileInfo"></param>
        public virtual void Render(string text, FileInformation fileInfo)
        {
            FileInfo = fileInfo;
            RawText = text;
            ConvertedText = CommonMarkConverter.Convert(text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.markdownViewer.Update();
        }

        /// <summary>
        /// Scroll the rendered panel vertically based on the given ration
        /// taken from Notepad++
        /// </summary>
        /// <param name="scrollRatio"></param>
        public abstract void ScrollByRatioVertically(double scrollRatio);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        protected string BuildHtml(string html = "", string title = "")
        {
            //
            if (title == "") title = this.assemblyTitle;
            //
            return $@"
<!DOCTYPE html>
<html>
    <head>
        <meta charset=""UTF-8"" />
        <meta name=""author"" content=""{this.assemblyTitle}"" />
        <title>{title}</title>
        <style type=""text/css"">
            {Resources.MarkdownViewerHTML}

            {this.markdownViewer.Options.HtmlCssStyle}
        </style>
      </head>
    <body>
        {html}
    </body>
</html>
            ";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void sendAsTextMail_Click(object sender, EventArgs e)
        {
            //Double-check
            if (IsOutlookInstalled())
            {
                Outlook.Application outlook = new Outlook.Application();
                Outlook.MailItem message = (Outlook.MailItem)outlook.CreateItem(Outlook.OlItemType.olMailItem);
                //
                message.Subject = this.FileInfo.FileName;
                message.BodyFormat = Outlook.OlBodyFormat.olFormatPlain;
                message.Body = RawText;
                message.Display();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void sendAsHTMLMail_Click(object sender, EventArgs e)
        {
            //Double-check
            if (IsOutlookInstalled())
            {
                Outlook.Application outlook = new Outlook.Application();
                Outlook.MailItem message = (Outlook.MailItem)outlook.CreateItem(Outlook.OlItemType.olMailItem);
                //
                message.Subject = this.FileInfo.FileName;
                message.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;
                message.HTMLBody = BuildHtml(ConvertedText, this.FileInfo.FileName);
                message.Display(true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void exportAsHTMLMenuItem_Click(object sender, EventArgs e)
        {
            //Save!
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //Default name of the file is the editor file name
            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(this.FileInfo.FileName) + ".html";
            //The current path
            saveFileDialog.InitialDirectory = this.markdownViewer.Notepad.GetCurrentDirectory();
            //
            saveFileDialog.Filter = "HTML files (*.html)|*.html|All files (*.*)|*.*";
            //
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName))
                {
                    string html = BuildHtml(ConvertedText, this.FileInfo.FileName);
                    try
                    {
                        html = XDocument.Parse(html).ToString();
                    }
                    catch { }
                    sw.WriteLine(html);
                }
                //Open if requested
                if (this.markdownViewer.Options.htmlOpenExport)
                {
                    Process.Start(saveFileDialog.FileName);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void exportAsPDFMenuItem_Click(object sender, EventArgs e)
        {
            //Save!
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //Default name of the file is the editor file name
            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(this.FileInfo.FileName) + ".pdf";
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
                //Set margins
                int[] margins = this.markdownViewer.Options.GetMargins();
                pdfConfig.MarginLeft = MilimiterToPoint(margins[0]);
                pdfConfig.MarginTop = MilimiterToPoint(margins[1]);
                pdfConfig.MarginRight = MilimiterToPoint(margins[2]);
                pdfConfig.MarginBottom = MilimiterToPoint(margins[3]);
                //Generate PDF and save
                PdfDocument pdf = PdfGenerator.GeneratePdf(BuildHtml(ConvertedText, this.FileInfo.FileName), pdfConfig, PdfGenerator.ParseStyleSheet(Resources.MarkdownViewerHTML));
                pdf.Save(saveFileDialog.FileName);

                //Open if requested
                if (this.markdownViewer.Options.pdfOpenExport)
                {
                    Process.Start(saveFileDialog.FileName);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void sendToPrinter_Click(object sender, EventArgs e)
        {
            //
            WebBrowser webBrowser = new WebBrowser();
            webBrowser.Parent = this;
            webBrowser.DocumentCompleted += (browser, webBrowserEvent) =>
            {
                ((WebBrowser)browser).Size = webBrowser.MaximumSize;
                ((WebBrowser)browser).ShowPrintPreviewDialog();
            };
            webBrowser.DocumentText = BuildHtml(ConvertedText, this.FileInfo.FileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void sendToClipboard_Click(object sender, EventArgs e)
        {
            ClipboardHelper.CopyToClipboard(BuildHtml(ConvertedText, this.FileInfo.FileName), ConvertedText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        protected int MilimiterToPoint(int mm)
        {
            return (int)(mm * 2.834646);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsOutlookInstalled()
        {
            try
            {
                if (Type.GetTypeFromProgID("Outlook.Application") != null)
                {
                    return true;
                }
            }
            catch { }

            return false;
        }
    }
}
