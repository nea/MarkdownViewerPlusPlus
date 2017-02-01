using Svg;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.WinForms;

/// <summary>
/// 
/// </summary>
namespace com.insanitydesign.MarkdownViewerPlusPlus.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public class MarkdownViewerHtmlPanel : HtmlPanel
    {
        /// <summary>
        /// 
        /// </summary>
        public MarkdownViewerHtmlPanel()
        {
            this.AllowDrop = false;
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IsContextMenuEnabled = false;
            this.Location = new System.Drawing.Point(0, 24);
            this.MinimumSize = new System.Drawing.Size(20, 20);
            this.Name = "markdownViewerHtmlPanel";
            this.Size = new System.Drawing.Size(284, 237);
            this.TabIndex = 0;
            //Add an SVG renderer
            this.ImageLoad += OnImageLoad;
        }

        /// <summary>
        /// Custom renderer for SVG images in the markdown as not supported natively.
        /// @see https://htmlrenderer.codeplex.com/wikipage?title=Rendering%20SVG%20images
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="imageLoadEvent"></param>
        protected void OnImageLoad(object sender, HtmlImageLoadEventArgs imageLoadEvent)
        {
            try
            {
                //Get some file information
                string src = imageLoadEvent.Src;
                Uri uri = new Uri(src);
                string extension = Path.GetExtension(src);
                
                //Check if local file or web resource
                switch (uri.Scheme.ToLowerInvariant())
                {
                    case "file":
                        //In case of a local file -> Load directly
                        if (extension != null && extension.Equals(".svg", StringComparison.OrdinalIgnoreCase))
                        {
                            ConvertSvgToBitmap(SvgDocument.Open<SvgDocument>(uri.LocalPath), imageLoadEvent);                            
                        }   
                        break;
                    case "http":
                    case "https":
                        //For web resources check extension and parameter, to fetch from e.g. "badge" creating sources
                        if ((extension != null && extension.Equals(".svg", StringComparison.OrdinalIgnoreCase))
                            || uri.ToString().Contains("svg="))
                        {
                            //In case of a web resource file -> Load async
                            using (WebClient webClient = new WebClient())
                            {
                                webClient.DownloadDataCompleted += (downloadSender, downloadEvent) => { OnDownloadDataCompleted(downloadEvent, imageLoadEvent); };
                                webClient.DownloadDataAsync(uri);
                            }
                        }
                        break;
                }
                
            } catch
            {
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="svgDocument"></param>
        /// <param name="imageLoadEvent"></param>
        protected Bitmap ConvertSvgToBitmap(SvgDocument svgDocument, HtmlImageLoadEventArgs imageLoadEvent = null)
        {
            Bitmap svgImage = new Bitmap((int)svgDocument.Width, (int)svgDocument.Height, PixelFormat.Format32bppArgb);
            svgDocument.Draw(svgImage);
            if(imageLoadEvent != null)
            {
                imageLoadEvent.Callback(svgImage);
            }
            return svgImage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="downloadEvent"></param>
        /// <param name="imageLoadEvent"></param>
        protected void OnDownloadDataCompleted(DownloadDataCompletedEventArgs downloadEvent, HtmlImageLoadEventArgs imageLoadEvent)
        {
            using (MemoryStream stream = new MemoryStream(downloadEvent.Result))
            {
                ConvertSvgToBitmap(SvgDocument.Open<SvgDocument>(stream), imageLoadEvent);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Text {
            get { return _text; }
            set {
                _text = value;
                //base.Text = value;
                if (!IsDisposed)
                {
                    _htmlContainer.SetHtml(_text, _baseCssData);
                    Redraw();
                }
            }
        }

        /// <summary>
        /// Scroll by the given ratio, calculated with max and page
        /// </summary>
        /// <param name="scrollRatio"></param>
        public void ScrollByRatioVertically(double scrollRatio)
        {
            if (!IsDisposed)
            {
                VerticalScroll.Value = (int)((VerticalScroll.Maximum - VerticalScroll.LargeChange) * scrollRatio);
                Redraw();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Redraw()
        {
            PerformLayout();
            Invalidate();
            InvokeMouseMove();
        }

        /// <summary>
        /// Release the SVGRenderer event
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            this.ImageLoad -= OnImageLoad;
            base.Dispose(disposing);
        }
    }
}
