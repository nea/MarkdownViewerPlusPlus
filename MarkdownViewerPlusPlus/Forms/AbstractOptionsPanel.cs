using System.Windows.Forms;

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
        protected MarkdownViewerConfiguration.MarkdownViewerOptions options;

        /// <summary>
        /// Empty constructor for Designer compatibility
        /// </summary>
        private AbstractOptionsPanel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public AbstractOptionsPanel(MarkdownViewerConfiguration.MarkdownViewerOptions options)
        {
            //
            this.options = options;
            //
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Dock = DockStyle.Fill;
            //
            InitializeComponent();
            //
            LoadOptions();
        }

        public virtual void LoadOptions()
        {            
        }

        public virtual void SaveOptions()
        {
        }

        protected virtual void InitializeComponent()
        {

        }
    }
}
