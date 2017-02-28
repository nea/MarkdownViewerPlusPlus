using PdfSharp;
using System;
using System.Linq;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public override void LoadOptions(Options options)
        {
            //Load options from enum
            this.cmbPDFOrientation.Items.AddRange(Enum.GetNames(typeof(PageOrientation)));
            //Set a default value
            this.cmbPDFOrientation.SelectedItem = PageOrientation.Portrait.ToString();
            //Now load
            this.cmbPDFOrientation.SelectedItem = options.pdfOrientation.ToString();
            //
            this.cmbPDFPageSize.Items.AddRange(Enum.GetNames(typeof(PageSize)));
            this.cmbPDFPageSize.Items.Remove(PageSize.Undefined.ToString());
            this.cmbPDFPageSize.SelectedItem = PageSize.A4.ToString();
            this.cmbPDFPageSize.SelectedItem = options.pdfPageSize.ToString();
            //Load margins
            int[] margins = options.GetMargins();
            this.numMarginLeft.Value = margins[0];
            this.numMarginTop.Value = margins[1];
            this.numMarginRight.Value = margins[2];
            this.numMarginBottom.Value = margins[3];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public override void SaveOptions(ref Options options)
        {
            PageOrientation pdfOrientation;
            if (Enum.TryParse<PageOrientation>(this.cmbPDFOrientation.SelectedItem.ToString(), out pdfOrientation))
            {
                options.pdfOrientation = pdfOrientation;
            }
            PageSize pdfPageSize;
            if (Enum.TryParse<PageSize>(this.cmbPDFPageSize.SelectedItem.ToString(), out pdfPageSize))
            {
                options.pdfPageSize = pdfPageSize;
            }
            //Save margins
            options.margins = this.numMarginLeft.Value + "," + this.numMarginTop.Value + "," + this.numMarginRight.Value + "," + this.numMarginBottom.Value;
        }
    }
}
