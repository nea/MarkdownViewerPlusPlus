using System;
using System.Reflection;
using System.Windows.Forms;

namespace com.insanitydesign.MarkdownViewerPlusPlus.Forms
{
    public partial class AboutDialog : Form
    {
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
  CommonMark.NET by Knagis
  PDFSharp by empira Software GmbH
  HTMLRenderer by ArthurHub
  SVG.NET by vvvv
  FontAwesome
  dcurtis

For more information, visit the website or checkout the included README.md
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVisit_Click(object sender, System.EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/nea/MarkdownViewerPlusPlus");
        }
    }
}
