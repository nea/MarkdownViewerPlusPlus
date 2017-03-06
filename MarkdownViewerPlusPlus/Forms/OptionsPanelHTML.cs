using static com.insanitydesign.MarkdownViewerPlusPlus.MarkdownViewerConfiguration;
/// <summary>
/// 
/// </summary>
namespace com.insanitydesign.MarkdownViewerPlusPlus.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class OptionsPanelHTML : AbstractOptionsPanel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public override void LoadOptions(Options options)
        {
            this.txtCssStyles.Text = options.HtmlCssStyle;
            this.chkOpenHTMLExport.Checked = options.htmlOpenExport;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public override void SaveOptions(ref Options options)
        {
            options.HtmlCssStyle = this.txtCssStyles.Text;
            options.htmlOpenExport = this.chkOpenHTMLExport.Checked;
        }
    }
}
