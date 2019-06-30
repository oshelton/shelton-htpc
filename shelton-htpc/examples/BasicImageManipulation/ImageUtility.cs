using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using ImageProcessor;

namespace BasicImageManipulation
{
    /// <summary>
    /// Helper class for some basic image manipulation operations.
    /// </summary>
    public static class ImageUtility
    {
        private static readonly double WidthTVAspectRatioFactor = 16.0 / 9.0;
        private static readonly double HeightTVAspectRatioFactor = 9.0 / 16.0;

        public static ImageFactory CropToTVAspectRatio(this ImageFactory processor)
        {
            //Crop down the width of the image.
            if (processor.Image.Height * WidthTVAspectRatioFactor < processor.Image.Width)
            {
                int widthToCut = (int) Math.Ceiling(processor.Image.Width - (processor.Image.Height * WidthTVAspectRatioFactor));
                return processor.Crop(new Rectangle(widthToCut / 2, 0, processor.Image.Width - widthToCut, processor.Image.Height));
            }
            else //Crop down the height of the image.
            {
                int heightToCut = (int)Math.Ceiling(processor.Image.Height - (processor.Image.Width * HeightTVAspectRatioFactor));
                return processor.Crop(new Rectangle(0, heightToCut / 2, processor.Image.Width, processor.Image.Height - heightToCut));
            }
        }

        public static ImageFactory ResizeImageTo4K(this ImageFactory processor)
        {
            return processor.Resize(new System.Drawing.Size(3840, 2160));
        }

        public static ImageFactory ResizeImageTo1080P(this ImageFactory processor)
        {
            return processor.Resize(new System.Drawing.Size(1920, 1080));
        }

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
