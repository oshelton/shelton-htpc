using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using LiteDB;

namespace LiteDBImageStorageExample
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
            List<BitmapImage> imagesToUse = new List<BitmapImage>();

            await Task.Run(() =>
            {
                MutableTestClass.RegisterEntityMap(BsonMapper.Global);

                using (var repo = new LiteRepository("test.db"))
                {
                    var collection = repo.Query<MutableTestClass>("test_objects").ToList();
                    if (collection.Count == 0)
                    {
                        repo.Insert<MutableTestClass>(new[] {
                            new MutableTestClass()
                            {
                                Id = Guid.NewGuid(),
                                Column = 0,
                                Row = 1,
                                Size = TileSize.SMALL,
                                Title = "Tile 1",
                                ImageId = "image1"
                            },
                            new MutableTestClass()
                            {
                                Id = Guid.NewGuid(),
                                Column = 1,
                                Row = 2,
                                Size = TileSize.LARGE,
                                Title = "Tile 2",
                                ImageId = "image2"
                            },
                        }, "test_objects");

                        repo.FileStorage.Upload("image1", @".\image1.jpg");
                        repo.FileStorage.Upload("image2", @".\image2.jpg");
                    }

                    var objects = repo.Query<MutableTestClass>("test_objects").ToList();

                    foreach (var obj in objects)
                    {
                        using (var outputStream = new MemoryStream())
                        {
                            repo.FileStorage.Download(obj.ImageId, outputStream);
                            outputStream.Position = 0;

                            BitmapImage image = new BitmapImage();
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = outputStream;
                            image.EndInit();
                            image.Freeze();

                            imagesToUse.Add(image);
                        }
                    }
                }
            });

            foreach (var image in imagesToUse)
            {
                ImageContainer.Children.Add(new Image() { Source = image });
            }
        }
    }
}
