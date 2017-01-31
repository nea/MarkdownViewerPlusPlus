using Kbg.NppPluginNET.PluginInfrastructure;
using System.IO;
using System.Reflection;
using System.Text;
/// <summary>
/// 
/// </summary>
namespace com.insanitydesign.MarkdownViewerPlusPlus
{
    /// <summary>
    /// 
    /// </summary>
    public class MarkdownViewerConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        protected string iniFilePath = null;

        /// <summary>
        /// 
        /// </summary>
        protected string assemblyName = "";

        /// <summary>
        /// 
        /// </summary>
        protected string synchronizeScrollingKey = "synchronizeScrolling";

        /// <summary>
        /// 
        /// </summary>
        public bool SynchronizeScrolling { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public MarkdownViewerConfiguration()
        {
            this.assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            Init();
        }

        /// <summary>
        /// Used for initing and re-initing
        /// </summary>
        public void Init()
        {
            //
            StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
            this.iniFilePath = sbIniFilePath.ToString();

            // if config path doesn't exist, we create it
            if (!Directory.Exists(iniFilePath))
            {
                Directory.CreateDirectory(iniFilePath);
            }

            //
            this.iniFilePath = Path.Combine(iniFilePath, this.assemblyName + ".ini");
            Load();
        }

        /// <summary>
        /// Load all configuration settings
        /// </summary>
        public void Load()
        {
            SynchronizeScrolling = (Win32.GetPrivateProfileInt(this.assemblyName, this.synchronizeScrollingKey, 0, iniFilePath) != 0);
        }

        /// <summary>
        /// Save all made changes to the configuration
        /// </summary>
        public void Save()
        {
            Win32.WritePrivateProfileString(this.assemblyName, this.synchronizeScrollingKey, SynchronizeScrolling ? "1" : "0", iniFilePath);
        }
    }
}
