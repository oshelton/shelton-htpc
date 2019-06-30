using ImageProcessor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BasicImageManipulation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BitmapImage hdImage = null;
            await Task.Run(() =>
            {
                var resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();

                using (var inputStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BasicImageManipulation.DSC_0389.jpg"))
                {
                    using (var processor = new ImageFactory())
                    {
                        processor.Load(inputStream);
                        var metadata = processor.ExifPropertyItems;

                        using (var croppedStream = new MemoryStream())
                        {
                            processor.CropToTVAspectRatio()
                                .Save(croppedStream);

                            processor.Load(croppedStream)
                                .ResizeImageTo4K()
                                .Save(@".\4k.jpg")
                                .Reset()
                                .ResizeImageTo1080P()
                                .Save(@".\1080p.jpg")
                                .Save(ref hdImage);
                        }
                    }
                }
            });

            hdImageView.Source = hdImage;
            GC.Collect();
        }
    }
}
