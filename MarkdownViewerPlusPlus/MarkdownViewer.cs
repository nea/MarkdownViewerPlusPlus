using com.insanitydesign.MarkdownViewerPlusPlus.Forms;
using com.insanitydesign.MarkdownViewerPlusPlus.Properties;
using Kbg.NppPluginNET;
using Kbg.NppPluginNET.PluginInfrastructure;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static com.insanitydesign.MarkdownViewerPlusPlus.MarkdownViewerConfiguration;
using static Kbg.NppPluginNET.PluginInfrastructure.Win32;

/// <summary>
/// 
/// </summary>
namespace com.insanitydesign.MarkdownViewerPlusPlus
{
    /// <summary>
    /// 
    /// </summary>
    public class MarkdownViewer
    {
        /// <summary>
        /// 
        /// </summary>
        public struct FileInformation
        {
            /// <summary>
            /// 
            /// </summary>
            public string FileDirectory { get; set; }
            /// <summary>
            /// 
            /// </summary>
            private string fileName;
            /// <summary>
            /// 
            /// </summary>
            public string FileName {
                get {
                    return this.fileName;
                }
                set {
                    this.fileName = value;
                    this.FileExtension = Path.GetExtension(this.fileName);
                    this.FileExtension = this.FileExtension.StartsWith(".") ? this.FileExtension.Substring(1, this.FileExtension.Length - 1) : this.FileExtension;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public string FileExtension { get; private set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IScintillaGateway Editor { get; protected set; }
        /// <summary>
        /// 
        /// </summary>
        public INotepadPPGateway Notepad { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public FileInformation FileInfo { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        protected AbstractRenderer renderer;

        /// <summary>
        /// 
        /// </summary>
        protected bool rendererVisible = false;

        /// <summary>
        /// 
        /// </summary>
        protected bool updateRenderer = true;

        /// <summary>
        /// 
        /// </summary>
        public int commandId = 0;

        /// <summary>
        /// 
        /// </summary>
        public int commandIdSynchronize = 2;

        /// <summary>
        /// 
        /// </summary>
        public int commandIdOptions = 4;

        /// <summary>
        /// 
        /// </summary>
        public int commandIdAbout = 5;

        /// <summary>
        /// 
        /// </summary>
        protected MarkdownViewerConfiguration configuration;
        
        /// <summary>
        /// 
        /// </summary>
        public Options Options {
            get {
                return this.configuration.options;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public MarkdownViewer()
        {
            //Get some global references to the editor and Notepad++ engines
            this.Editor = new ScintillaGateway(PluginBase.GetCurrentScintilla());
            this.Notepad = new NotepadPPGateway();
            //Init the actual renderer
            this.renderer = new MarkdownViewerRenderer(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notification"></param>
        public void OnNotification(ScNotification notification)
        {
            //Listen to any UI update to get informed about all file changes, chars added/removed etc.
            if (this.renderer.Visible)
            {
                //Check for updates
                if (notification.Header.Code == (uint)SciMsg.SCN_UPDATEUI)
                {
                    //Update the view
                    Update((notification.Updated & (uint)SciMsg.SC_UPDATE_V_SCROLL) != 0);
                }
                else if (notification.Header.Code == (uint)NppMsg.NPPN_BUFFERACTIVATED)
                {
                    //Update the scintilla handle in all cases to keep track of which instance is active
                    UpdateEditorInformation();
                    this.Editor.CurrentBufferID = notification.Header.IdFrom;
                    Update(true, true);
                }
                else if (notification.Header.Code == (uint)SciMsg.SCN_MODIFIED && !this.updateRenderer)
                {
                    bool isInsert = (notification.ModificationType & (uint)SciMsg.SC_MOD_INSERTTEXT) != 0;
                    bool isDelete = (notification.ModificationType & (uint)SciMsg.SC_MOD_DELETETEXT) != 0;

                    //Track if any text modifications have been made
                    this.updateRenderer = isInsert || isDelete;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateScrollBar"></param>
        /// <param name="updateRenderer"></param>
        public void Update(bool updateScrollBar = false, bool updateRenderer = false)
        {
            //Validate that the current file may be rendered
            if (this.configuration.ValidateFileExtension(this.FileInfo.FileExtension, this.FileInfo.FileName))
            {
                //Update the view
                this.updateRenderer = updateRenderer ? updateRenderer : this.updateRenderer;
                UpdateMarkdownViewer();
                //Update the scroll bar of the Viewer Panel only in case of vertical scrolls
                if (this.configuration.options.synchronizeScrolling && updateScrollBar)
                {
                    UpdateScrollBar();
                }
            }
            else
            {
                this.renderer.Render($@"<p>
Your configuration settings do not include the currently selected file extension.<br />
The rendered file extensions are <b>'{this.configuration.options.fileExtensions}'</b>.<br />
The current file is <i>'{this.FileInfo.FileName}'</i>.
                </p>", this.FileInfo);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void UpdateScrollBar()
        {
            try
            {
                ScrollInfo scrollInfo = this.Editor.GetScrollInfo(ScrollInfoMask.SIF_RANGE | ScrollInfoMask.SIF_TRACKPOS | ScrollInfoMask.SIF_PAGE, ScrollInfoBar.SB_VERT);
                var scrollRatio = (double)scrollInfo.nTrackPos / (scrollInfo.nMax - scrollInfo.nPage);
                this.renderer.ScrollByRatioVertically(scrollRatio);
            }
            catch { }            
        }

        /// <summary>
        /// 
        /// </summary>
        public void CommandMenuInit()
        {
            this.configuration = new MarkdownViewerConfiguration();

            //Register our commands
            PluginBase.SetCommand(this.commandId, Main.PluginName, MarkdownViewerCommand, new ShortcutKey(true, false, true, System.Windows.Forms.Keys.M));
            //Separator
            PluginBase.SetCommand(this.commandId + 1, "---", null);
            //Synchronized scrolling
            PluginBase.SetCommand(this.commandIdSynchronize, "Synchronize scrolling (Editor -> Viewer)", SynchronizeScrollingCommand, this.configuration.options.synchronizeScrolling);
            //Separator
            PluginBase.SetCommand(this.commandIdSynchronize + 1, "---", null);
            //Options
            PluginBase.SetCommand(this.commandIdOptions, "Options", OptionsCommand);
            //About
            PluginBase.SetCommand(this.commandIdAbout, "About", AboutCommand);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OptionsCommand()
        {
            using (MarkdownViewerOptions options = new MarkdownViewerOptions(ref this.configuration))
            {
                if (options.ShowDialog(Control.FromHandle(PluginBase.GetCurrentScintilla())) == DialogResult.OK)
                {
                    //Update after something potentially changed in the settings dialog
                    Update(true, true);
                }
            }
        }

        /// <summary>
        /// Show the AboutDialog as modal to the Notepad++ window
        /// </summary>
        private void AboutCommand()
        {
            using (AboutDialog about = new AboutDialog())
            {
                about.ShowDialog(Control.FromHandle(PluginBase.GetCurrentScintilla()));
            }
        }

        /// <summary>
        /// Check/Uncheck the configuration item
        /// </summary>
        protected void SynchronizeScrollingCommand()
        {
            this.configuration.options.synchronizeScrolling = !this.configuration.options.synchronizeScrolling;
            Win32.CheckMenuItem(Win32.GetMenu(PluginBase.nppData._nppHandle), PluginBase._funcItems.Items[this.commandIdSynchronize]._cmdID, Win32.MF_BYCOMMAND | (this.configuration.options.synchronizeScrolling ? Win32.MF_CHECKED : Win32.MF_UNCHECKED));
            if (this.configuration.options.synchronizeScrolling)
            {
                UpdateScrollBar();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetToolBarIcon()
        {
            toolbarIcons toolbarIcons = new toolbarIcons();
            toolbarIcons.hToolbarBmp = Resources.markdown_16x16_solid.GetHbitmap();
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(toolbarIcons));
            Marshal.StructureToPtr(toolbarIcons, pTbIcons, false);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON, PluginBase._funcItems.Items[this.commandId]._cmdID, pTbIcons);
            Marshal.FreeHGlobal(pTbIcons);
        }

        /// <summary>
        /// 
        /// </summary>
        public void MarkdownViewerCommand()
        {
            bool visibility = !this.renderer.Visible;
            ToggleToolbarIcon(visibility);
            //Show
            if (visibility)
            {
                UpdateEditorInformation();
                Update(true, true);
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        protected void UpdateEditorInformation()
        {
            this.Editor.SetScintillaHandle(PluginBase.GetCurrentScintilla());
            this.FileInfo = new FileInformation() {
                FileName = this.Notepad.GetCurrentFileName(),
                FileDirectory = this.Notepad.GetCurrentDirectory()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="show"></param>
        /// <returns></returns>
        public void ToggleToolbarIcon(bool show = true)
        {
            //To show or not to show
            NppMsg msg = show ? NppMsg.NPPM_DMMSHOW : NppMsg.NPPM_DMMHIDE;
            //
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)msg, 0, this.renderer.Handle);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK, PluginBase._funcItems.Items[this.commandId]._cmdID, show ? 1 : 0);
            //
            this.rendererVisible = this.renderer.Visible;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void UpdateMarkdownViewer()
        {
            try
            {
                if (this.updateRenderer)
                {
                    this.updateRenderer = false;
                    this.renderer.Render(this.Editor.GetText(this.Editor.GetLength() + 1), this.FileInfo);
                }
            }
            catch
            {
                this.renderer.Render("<p>Couldn't render the currently selected file!</p>", this.FileInfo);
            }
        }

        /// <summary>
        /// Save the configuration
        /// </summary>
        public void PluginCleanUp()
        {
            this.configuration.Save();
        }
    }
}
