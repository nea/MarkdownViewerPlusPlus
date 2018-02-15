using System;
using System.Reflection;
using System.Windows.Forms;

/// <summary>
/// 
/// </summary>
namespace com.insanitydesign.MarkdownViewerPlusPlus.Forms
{
    /// <summary>
    /// About Dialog for this plugin with basic information and link to the website.
    /// </summary>
    public partial class AboutDialog : Form
    {
        /// <summary>
        /// Init the about dialog text from the Assembly Information
        /// </summary>
        public AboutDialog()
        {
            InitializeComponent();

            //Get the assembly information for the About dialog
            string title = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false)).Title;
            string description = ((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyDescriptionAttribute), false)).Description;
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //About Text
            this.lblAbout.Text = $@"{title}

{description}

Version: {version}

Many thanks to:
  Notepad++ PluginPack.net by kbilsted
  Markdig by lunet-io
  PDFSharp by empira Software GmbH
  HTMLRenderer by ArthurHub
  SVG.NET by vvvv
  FontAwesome
  dcurtis

For more information, visit the website or check the included README.md
            ";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Visit GitHub
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVisit_Click(object sender, System.EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/nea/MarkdownViewerPlusPlus");
        }
    }
}
