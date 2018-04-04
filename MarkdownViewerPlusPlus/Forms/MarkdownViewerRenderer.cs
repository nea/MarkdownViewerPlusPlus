using System;
using System.IO;
using Svg;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Threading;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using static com.insanitydesign.MarkdownViewerPlusPlus.MarkdownViewer;

/// <summary>
/// 
/// </summary>
namespace com.insanitydesign.MarkdownViewerPlusPlus.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public class MarkdownViewerRenderer : AbstractRenderer
    {
        /// <summary>
        /// 
        /// </summary>
        public MarkdownViewerHtmlPanel markdownViewerHtmlPanel;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="markdownViewer"></param>
        public MarkdownViewerRenderer(MarkdownViewer markdownViewer) : base(markdownViewer)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Init()
        {
            base.Init();
            //
            this.markdownViewerHtmlPanel = new MarkdownViewerHtmlPanel();
            //Add a custom image loader
            this.markdownViewerHtmlPanel.ImageLoad += OnImageLoad;
            //Add to view
            this.Controls.Add(this.markdownViewerHtmlPanel);
            this.Controls.SetChildIndex(this.markdownViewerHtmlPanel, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fileInfo"></param>
        public override void Render(string text, FileInformation fileInfo)
        {
            base.Render(text, fileInfo);
            this.markdownViewerHtmlPanel.Text = BuildHtml(ConvertedText, fileInfo.FileName);
        }

        /// <summary>
        /// Scroll the rendered panel vertically based on the given ration
        /// taken from Notepad++
        /// </summary>
        /// <param name="scrollRatio"></param>
        public override void ScrollByRatioVertically(double scrollRatio)
        {
            this.markdownViewerHtmlPanel.ScrollByRatioVertically(scrollRatio);
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
                        //In case of a local file -> Try to load it directly
                        imageLoadEvent.Handled = true; //Tell the event it was handled, so no error border is drawn
                        ThreadPool.QueueUserWorkItem(state => LoadImageFromFile(src, imageLoadEvent));
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
                                imageLoadEvent.Handled = true; //Tell the event it was handled, so no error border is drawn
                                webClient.DownloadDataCompleted += (downloadSender, downloadEvent) => { OnDownloadDataCompleted(downloadEvent, imageLoadEvent); };
                                webClient.DownloadDataAsync(uri);
                            }
                        }
                        break;
                }

            }
            catch
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="imageLoadEvent"></param>
        protected void LoadImageFromFile(string src, HtmlImageLoadEventArgs imageLoadEvent)
        {
            try
            {
                Uri uri = new Uri(src);
                //Try to load the file as Image from file
                //Remove the scheme first
                string srcWithoutScheme = src;
                int i = srcWithoutScheme.IndexOf(':');
                if (i > 0) srcWithoutScheme = srcWithoutScheme.Substring(i + 1).TrimStart('/');
                //If not absolute, add the current file path
                if (!Path.IsPathRooted(srcWithoutScheme))
                {
                    uri = new Uri(@"file:///" + this.FileInfo.FileDirectory + "/" + srcWithoutScheme);
                }

                //For SVG images: Convert to Bitmap
                string extension = Path.GetExtension(src);
                if (extension != null && extension.Equals(".svg", StringComparison.OrdinalIgnoreCase))
                {
                    ConvertSvgToBitmap(SvgDocument.Open<SvgDocument>(uri.LocalPath), imageLoadEvent);
                }
                else
                {
                    //Load uri, 8, 1
                    imageLoadEvent.Callback((Bitmap)Image.FromFile(uri.LocalPath, true));
                }
            }
            catch { } //Not able to handle, refer back to orginal process
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="svgDocument"></param>
        /// <param name="imageLoadEvent"></param>
        protected Bitmap ConvertSvgToBitmap(SvgDocument svgDocument, HtmlImageLoadEventArgs imageLoadEvent)
        {
            Bitmap svgImage = new Bitmap((int)svgDocument.Width, (int)svgDocument.Height, PixelFormat.Format32bppArgb);
            svgDocument.Draw(svgImage);
            imageLoadEvent.Callback(svgImage);
            imageLoadEvent.Handled = true;
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
        /// Release the custom loader
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (this.markdownViewerHtmlPanel != null)
            {
                this.markdownViewerHtmlPanel.ImageLoad -= OnImageLoad;
            }
            base.Dispose(disposing);
        }
    }
}
