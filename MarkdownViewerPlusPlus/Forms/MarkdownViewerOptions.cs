using System.Reflection;
using System.Windows.Forms;

/// <summary>
/// 
/// </summary>
namespace com.insanitydesign.MarkdownViewerPlusPlus.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MarkdownViewerOptions : Form
    {
        /// <summary>
        /// 
        /// </summary>
        protected AbstractOptionsPanel optionsGeneral;
        /// <summary>
        /// 
        /// </summary>
        protected AbstractOptionsPanel optionsHTML;

        /// <summary>
        /// 
        /// </summary>
        protected MarkdownViewerConfiguration configuration;

        /// <summary>
        /// 
        /// </summary>
        public MarkdownViewerOptions(MarkdownViewerConfiguration configuration)
        {
            //
            this.configuration = configuration;
            this.optionsGeneral = new OptionsGeneral(this.configuration.Options);
            this.optionsHTML = new OptionsHTML(this.configuration.Options);
            //
            InitializeComponent();
            //
            this.treeOptions.NodeMouseClick += treeOptions_NodeMouseClick;
            //Start with the general options panel
            this.splitOptions.Panel2.Controls.Add(this.optionsGeneral);
            this.treeOptions.Select();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="treeNodeEvent"></param>
        protected void treeOptions_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs treeNodeEvent)
        {            
            //Remove old (if any)
            if(this.splitOptions.Panel2.Controls.Count > 0)
            {
                this.splitOptions.Panel2.Controls.RemoveAt(0);
            }
            //
            FieldInfo optionsPanel = this.GetType().GetField(treeNodeEvent.Node.Tag.ToString(), BindingFlags.Instance | BindingFlags.NonPublic);
            //Add selected options panel
            this.splitOptions.Panel2.Controls.Add((UserControl)optionsPanel.GetValue(this));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOptionsCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOptionsSave_Click(object sender, System.EventArgs e)
        {
            //TODO: Set all values
            this.optionsGeneral.SaveOptions();
            //
            this.configuration.Save();
        }
    }
}
