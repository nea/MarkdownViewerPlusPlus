using com.insanitydesign.MarkdownViewerPlusPlus.Properties;
using Kbg.NppPluginNET;
using Kbg.NppPluginNET.PluginInfrastructure;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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
            _nppTbData.dlgID = this.markdownViewer.CommandId;
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
        /// <param name="html"></param>
        public abstract void Render(string html);

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
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public abstract string GetHtml(string title = "", string html = null);
    }
}
