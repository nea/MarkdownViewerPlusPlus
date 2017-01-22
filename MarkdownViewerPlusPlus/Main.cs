using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Kbg.NppPluginNET.PluginInfrastructure;
using com.insanitydesign.MarkdownViewerPlusPlus;

/// <summary>
/// 
/// </summary>
namespace Kbg.NppPluginNET
{
    /// <summary>
    /// 
    /// </summary>
    class Main
    {
        /// <summary>
        /// 
        /// </summary>
        public const string PluginName = "MarkdownViewerPlusPlus";

        /// <summary>
        /// "Singleton" for this plugin of the actual implementation
        /// </summary>
        private static MarkdownViewer MarkdownViewer = new MarkdownViewer();

        /// <summary>
        /// This method is invoked whenever something is happening in notepad++
        /// use eg. as
        /// if (notification.Header.Code == (uint)NppMsg.NPPN_xxx)
        /// { ... }
        /// or
        ///
        /// if (notification.Header.Code == (uint)SciMsg.SCNxxx)
        /// { ... }
        /// </summary>
        /// <param name="notification"></param>
        public static void OnNotification(ScNotification notification)
        {
            MarkdownViewer.OnNotification(notification);
        }

        /// <summary>
        /// 
        /// </summary>
        internal static void CommandMenuInit()
        {
            MarkdownViewer.CommandMenuInit();
        }

        /// <summary>
        /// 
        /// </summary>
        internal static void SetToolBarIcon()
        {
            MarkdownViewer.SetToolBarIcon();
        }

        /// <summary>
        /// 
        /// </summary>
        internal static void PluginCleanUp()
        {
            //Nothing to see here... go on ^^
        }
    }
}