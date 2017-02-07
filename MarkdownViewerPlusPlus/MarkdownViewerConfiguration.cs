using Kbg.NppPluginNET.PluginInfrastructure;
using PdfSharp;
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
        public struct Options
        {
            /// <summary>
            /// 
            /// </summary>
            public bool synchronizeScrolling;
            /// <summary>
            /// 
            /// </summary>
            public string fileExtensions;
            /// <summary>
            /// 
            /// </summary>
            public bool inclNewFiles;
            /// <summary>
            /// 
            /// </summary>
            public string htmlCssStyle;
            /// <summary>
            /// 
            /// </summary>
            public PageOrientation pdfOrientation;
        }

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
        public Options options;

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
            //Grab ini file settings based on struct members
            this.options = new Options();
            //Unbox/Box magic to set structs
            object options = this.options;
            foreach (FieldInfo field in this.options.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if(field.FieldType == typeof(bool))
                {
                    field.SetValue(options, (Win32.GetPrivateProfileInt(this.assemblyName, field.Name, 0, iniFilePath) != 0));
                } else if (field.FieldType == typeof(string))
                {
                    StringBuilder sbFieldValue = new StringBuilder(2048);
                    Win32.GetPrivateProfileString(this.assemblyName, field.Name, "", sbFieldValue, 2048, iniFilePath);
                    field.SetValue(options, sbFieldValue.ToString());
                }                
            }
            //Unbox/Box magic to set structs
            this.options = (Options)options;
        }

        /// <summary>
        /// Save all made changes to the configuration
        /// </summary>
        public void Save()
        {
            //Save ini file settings based on struct members
            foreach (FieldInfo field in this.options.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                var value = field.GetValue(this.options);
                value = value != null ? value : "";
                if (field.FieldType == typeof(bool))
                {
                    Win32.WritePrivateProfileString(this.assemblyName, field.Name, ((bool)value) ? "1" : "0", iniFilePath);
                }
                else if (field.FieldType == typeof(string))
                {
                    Win32.WritePrivateProfileString(this.assemblyName, field.Name, value.ToString(), iniFilePath);
                }
            }
        }
    }
}
