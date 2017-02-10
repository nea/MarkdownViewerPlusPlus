using com.insanitydesign.MarkdownViewerPlusPlus.Properties;
using com.insanitydesign.MarkdownViewerPlusPlus.Windows;
using Kbg.NppPluginNET;
using Kbg.NppPluginNET.PluginInfrastructure;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static com.insanitydesign.MarkdownViewerPlusPlus.Windows.WindowsMessage;

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
        protected string originalText;
        
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
            //Register dockable window and hide initially (TODO: Add to settings to restore)
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMHIDE, 0, this.Handle);
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
        /// <param name="fileName"></param>
        public virtual void Render(string text, string fileName = "")
        {
            SetText(text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected abstract void exportAsHTMLMenuItem_Click(object sender, EventArgs e);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected abstract void exportAsPDFMenuItem_Click(object sender, EventArgs e);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.markdownViewer.UpdateMarkdownViewer();
        }

        /// <summary>
        /// Return the originally rendered text
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            return this.originalText;
        }

        /// <summary>
        /// Set the originally to be rendered text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public void SetText(string text)
        {
            this.originalText = text;
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
        <meta charset='UTF-8'>
        <meta name='author' content='{this.assemblyTitle}'>
        <title>{title}</title>
        <style type='text/css'>
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
    }
}
