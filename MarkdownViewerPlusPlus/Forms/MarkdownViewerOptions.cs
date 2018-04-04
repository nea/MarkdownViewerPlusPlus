using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static com.insanitydesign.MarkdownViewerPlusPlus.MarkdownViewerConfiguration;

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
        protected MarkdownViewerConfiguration configuration;

        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<string, AbstractOptionsPanel> optionPanels = new Dictionary<string, AbstractOptionsPanel>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        protected delegate void SaveHandler(ref Options options);

        /// <summary>
        /// 
        /// </summary>
        protected SaveHandler SaveEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        protected delegate void LoadHandler(Options options);

        /// <summary>
        /// 
        /// </summary>
        protected LoadHandler LoadEvent;

        /// <summary>
        /// 
        /// </summary>
        public MarkdownViewerOptions(ref MarkdownViewerConfiguration configuration)
        {
            //
            this.configuration = configuration;
            //
            InitializeComponent();
            //Iterate over all menu items and register their classes            
            string thisNamespace = typeof(MarkdownViewerOptions).Namespace;
            foreach (TreeNode node in this.treeOptions.Nodes)
            {
                AbstractOptionsPanel optionsPanel = (AbstractOptionsPanel)Activator.CreateInstance(Type.GetType(thisNamespace + "." + node.Tag.ToString()));
                this.SaveEvent += optionsPanel.SaveOptions;
                this.LoadEvent += optionsPanel.LoadOptions;
                //Add to map to store for changes
                this.optionPanels.Add(node.Tag.ToString(), optionsPanel);
            }
            //
            this.treeOptions.AfterSelect += treeOptions_AfterSelect;
            //Start with the general options panel
            this.splitOptions.Panel2.Controls.Add(this.optionPanels.First().Value);
            this.treeOptions.Select();
            //Set the according dialog result to their respective buttons
            this.btnOptionsCancel.DialogResult = DialogResult.Cancel;
            this.btnOptionsSave.DialogResult = DialogResult.OK;
            //
            this.LoadEvent(this.configuration.options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="treeNodeEvent"></param>
        protected void treeOptions_AfterSelect(object sender, TreeViewEventArgs treeNodeEvent)
        {
            //Remove old (if any)
            if (this.splitOptions.Panel2.Controls.Count > 0)
            {
                this.splitOptions.Panel2.Controls.RemoveAt(0);
            }
            //Add selected options panel
            AbstractOptionsPanel optionPanel = this.optionPanels.Where(entry => entry.Key == treeNodeEvent.Node.Tag.ToString()).First().Value;
            this.splitOptions.Panel2.Controls.Add(optionPanel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOptionsCancel_Click(object sender, System.EventArgs e)
        {
            this.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOptionsSave_Click(object sender, System.EventArgs e)
        {
            //Fire the save event for all listeners
            this.SaveEvent(ref this.configuration.options);
            //Save to init
            this.configuration.Save();            
            //Close the options dialog when all has been done
            this.Dispose();
        }
    }
}
