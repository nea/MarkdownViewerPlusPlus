using PdfSharp;
using System;
using static com.insanitydesign.MarkdownViewerPlusPlus.MarkdownViewerConfiguration;
/// <summary>
/// 
/// </summary>
namespace com.insanitydesign.MarkdownViewerPlusPlus.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class OptionsPanelPDF : AbstractOptionsPanel
    {
        public override void LoadOptions(Options options)
        {
            //Load options from enum
            this.cmbPDFOrientation.Items.AddRange(Enum.GetNames(typeof(PageOrientation)));
            //Set a default value (portrait is fine)
            this.cmbPDFOrientation.SelectedItem = PageOrientation.Portrait;
        }

        public override void SaveOptions(ref Options options)
        {
        }
    }
}
