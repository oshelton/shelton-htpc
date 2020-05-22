using ImageProcessor;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SheltonHTPC.Common.Utils
{
    /// <summary>
    /// Utility class that extends ImageProcessor's ImageFactory class with some addiitonal features.
    /// </summary>
    public static class ImageFactoryExtensions
    {
        public static readonly int UhdWidth = 3840;
        public static readonly int UhdHeight = 2160;

        public static readonly int HdWidth = 1920;
        public static readonly int HdHeight = 1080;

        private static readonly double WidthTVAspectRatioFactor = 16.0 / 9.0;
        private static readonly double HeightTVAspectRatioFactor = 9.0 / 16.0;

        /// <summary>
        /// Crop the passed in image processor to the standard tv 16:9 aspect ratio, centering the resulting image in the remaining space depending on which axis was cropped.
        /// </summary>
        public static ImageFactory CropToTVAspectRatio(this ImageFactory processor)
        {
            //Crop down the width of the image.
            if (processor.Image.Height * WidthTVAspectRatioFactor < processor.Image.Width)
            {
                int widthToCut = (int)Math.Ceiling(processor.Image.Width - (processor.Image.Height * WidthTVAspectRatioFactor));
                return processor.Crop(new Rectangle(widthToCut / 2, 0, processor.Image.Width - widthToCut, processor.Image.Height));
            }
            else //Crop down the height of the image.
            {
                int heightToCut = (int)Math.Ceiling(processor.Image.Height - (processor.Image.Width * HeightTVAspectRatioFactor));
                return processor.Crop(new Rectangle(0, heightToCut / 2, processor.Image.Width, processor.Image.Height - heightToCut));
            }
        }

        /// <summary>
        /// Resize an image to the standard Uhd/4k (3840x2160) resolution.
        /// </summary>
        public static ImageFactory ResizeImageToUhd(this ImageFactory processor)
        {
            return processor.Resize(new System.Drawing.Size(3840, 2160));
        }

        /// <summary>
        /// Resize an image to the standard Hd/1080p (1920x1080) resolution.
        /// </summary>
        public static ImageFactory ResizeImageToHd(this ImageFactory processor)
        {
            return processor.Resize(new System.Drawing.Size(1920, 1080));
        }

        /// <summary>
        /// Resizes an image to a particular height preserving the aspect ratio.
        /// </summary>
        public static ImageFactory ResizeImageToHeight(this ImageFactory processor, int height)
        {
            double factor = processor.Image.Height / processor.Image.Width;
            return processor.Resize(new System.Drawing.Size((int) (height / factor), height));
        }

        /// <summary>
        /// Resizes an image to a particular height preserving the aspect ratio.
        /// </summary>
        public static ImageFactory ResizeImageToWidth(this ImageFactory processor, int width)
        {
            double factor = processor.Image.Width / processor.Image.Height;
            return processor.Resize(new System.Drawing.Size(width, (int) (width / factor)));
        }

        /// <summary>
        /// Create a  WPF BitmapImage from the passed in ImageFactory.  
        /// If imagePortionRect is not null just a portion of the image will be used to create the resulting BitmapImage.
        /// </summary>
        public static ImageFactory Save(this ImageFactory processor, ref BitmapImage toImage, Int32Rect? imagePortionRect = null)
        {
            using (var outputStream = new MemoryStream())
            {
                processor.Save(outputStream);

                toImage = new BitmapImage();
                toImage.BeginInit();
                if (imagePortionRect != null)
                    toImage.SourceRect = imagePortionRect.Value;
                toImage.StreamSource = outputStream;
                toImage.CacheOption = BitmapCacheOption.OnLoad;
                toImage.EndInit();
                toImage.Freeze();

                return processor;
            }
        }
    }
}
