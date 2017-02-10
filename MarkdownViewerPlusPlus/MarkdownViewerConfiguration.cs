using Kbg.NppPluginNET.PluginInfrastructure;
using PdfSharp;
using System;
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
            private string htmlCssStyle;
            /// <summary>
            /// 
            /// </summary>
            public string HtmlCssStyle {
                get {
                    if (htmlCssStyle == null) return "";
                    return htmlCssStyle.Replace(@" \n ", Environment.NewLine);
                }
                set {
                    htmlCssStyle = value.Replace(Environment.NewLine, @" \n ");
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public PageOrientation pdfOrientation;
            /// <summary>
            /// 
            /// </summary>
            public PageSize pdfPageSize;
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
            foreach (FieldInfo field in this.options.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (field.FieldType == typeof(bool))
                {
                    field.SetValue(options, (Win32.GetPrivateProfileInt(this.assemblyName, field.Name, 0, iniFilePath) != 0));
                }
                else if (field.FieldType == typeof(string))
                {
                    StringBuilder sbFieldValue = new StringBuilder(32767);
                    Win32.GetPrivateProfileString(this.assemblyName, field.Name, "", sbFieldValue, 32767, iniFilePath);
                    field.SetValue(options, sbFieldValue.ToString());
                }
                else if (field.FieldType.IsEnum)
                {
                    StringBuilder sbFieldValue = new StringBuilder(Win32.MAX_PATH);
                    Win32.GetPrivateProfileString(this.assemblyName, field.Name, "", sbFieldValue, Win32.MAX_PATH, iniFilePath);
                    try
                    {
                        field.SetValue(options, Enum.Parse(field.FieldType, sbFieldValue.ToString()));
                    }
                    catch { }
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
                else if (field.FieldType == typeof(string) || field.FieldType.IsEnum)
                {
                    Win32.WritePrivateProfileString(this.assemblyName, field.Name, value.ToString(), iniFilePath);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool ValidateFileExtension(string fileExtension, string fileName = "")
        {
            //Nothing set -> Render all
            if (this.options.fileExtensions == null || this.options.fileExtensions == "") return true;
            //Something set but nothing given, check for "new " files (dirty dirty ^^)
            if (fileExtension == null || fileExtension == "") return this.options.inclNewFiles && fileName.StartsWith("new ");
            //Otherwise check
            return this.options.fileExtensions.Contains(fileExtension);
        }
    }
}
