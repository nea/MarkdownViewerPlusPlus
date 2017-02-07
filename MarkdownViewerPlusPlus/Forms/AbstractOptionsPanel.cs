using System;
using System.Windows.Forms;
using static com.insanitydesign.MarkdownViewerPlusPlus.MarkdownViewerConfiguration;

/// <summary>
/// 
/// </summary>
namespace com.insanitydesign.MarkdownViewerPlusPlus.Forms
{
    /// <summary>
    /// Abstract class not being abstract for Designer compatibility
    /// </summary>
    public class AbstractOptionsPanel : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public AbstractOptionsPanel()
        {
            //
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Dock = DockStyle.Fill;
            //
            InitializeComponent();
        }

        /// <summary>
        /// Load all options from the local Options instance
        /// onto the panel.
        /// </summary>
        public virtual void LoadOptions(Options options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Save the made selections/entries of the panel
        /// in the local Options instance.
        /// </summary>
        public virtual void SaveOptions(ref Options options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void InitializeComponent()
        {
            throw new NotImplementedException();
        }
    }
}
