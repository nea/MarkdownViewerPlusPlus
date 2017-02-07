using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using static com.insanitydesign.MarkdownViewerPlusPlus.MarkdownViewerConfiguration;

/// <summary>
/// 
/// </summary>
namespace com.insanitydesign.MarkdownViewerPlusPlus.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class OptionsPanelGeneral : AbstractOptionsPanel
    {
        /// <summary>
        /// 
        /// </summary>
        protected string msgFileExtensions = "Add a list of comma-separated file extensions (e.g. \'log,txt,html\'). Empty the box for \'All files\'.";

        /// <summary>
        /// 
        /// </summary>
        protected string regExFileExtensions = "^([a-zA-Z,]*)$";

        /// <summary>
        /// 
        /// </summary>
        public OptionsPanelGeneral()
        {
            //
            this.txtFileExtensions.Enter += txtFileExtensions_Enter;
            this.txtFileExtensions.Leave += txtFileExtensions_Leave;
            this.txtFileExtensions.Validating += txtFileExtensions_Validating;
        }

        /// <summary>
        /// Validate that the file extensions field has a correct value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtFileExtensions_Validating(object sender, CancelEventArgs e)
        {
            if (!Regex.IsMatch(this.txtFileExtensions.Text, this.regExFileExtensions, RegexOptions.IgnoreCase))
            {
                MessageBox.Show(string.Format("Please check \'{0}\'\r\n" + this.msgFileExtensions, this.lblFileExtensions.Text), "Warning!", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFileExtensions_Enter(object sender, EventArgs e)
        {
            this.toolTipFileExtensions.Show(this.msgFileExtensions, this.txtFileExtensions, this.lblFileExtensions.Width, -75);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFileExtensions_Leave(object sender, EventArgs e)
        {
            this.toolTipFileExtensions.Hide(this.txtFileExtensions);

        }

        /// <summary>
        /// 
        /// </summary>
        public override void SaveOptions(ref Options options)
        {
            options.inclNewFiles = this.chkBoxNewFiles.Checked;
            options.fileExtensions = this.txtFileExtensions.Text;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void LoadOptions(Options options)
        {
            this.chkBoxNewFiles.Checked = options.inclNewFiles;
            this.txtFileExtensions.Text = options.fileExtensions;
        }
    }
}
