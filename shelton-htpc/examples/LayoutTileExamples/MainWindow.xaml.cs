using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LayoutTileExamples
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
        private enum TileSize
        {
            Normal,
            Large,
            ExtraLarge
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundContainer.RenderTransform = new TranslateTransform();
            CreateBackgrounds();
            CreateTiles();
        }

        private void CreateBackgrounds()
        {
            var background1 = new Image
            {
                Width = App.ViewWidth,
                Height = App.ViewHeight,
                Stretch = Stretch.UniformToFill,
                Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/test_image.jpg"))
            };
            BackgroundContainer.Children.Add(background1);

            var background2 = new Image
            {
                Width = App.ViewWidth,
                Height = App.ViewHeight,
                Stretch = Stretch.UniformToFill,
                Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/test_image2.jpg"))
            };
            BackgroundContainer.Children.Add(background2);
        }

        private void CreateTiles()
        {
			TileSceneContainer.RenderTransform = new TranslateTransform();

            var scene1 = new Canvas();
            scene1.Width = App.ViewWidth;
            scene1.Height = App.HeightContentArea;

            scene1.Children.Add(CreateRectangleTile(0, 0, Brushes.AliceBlue));
            scene1.Children.Add(CreateRectangleTile(0, 1, Brushes.BurlyWood));
            scene1.Children.Add(CreateRectangleTile(0, 2, Brushes.Coral));
            scene1.Children.Add(CreateRectangleTile(0, 3, Brushes.CornflowerBlue));

            scene1.Children.Add(CreateRectangleTile(4, 0, Brushes.Firebrick, TileSize.Large));

            scene1.Children.Add(CreateRectangleTile(3, 0, Brushes.DimGray));
            scene1.Children.Add(CreateRectangleTile(3, 1, Brushes.ForestGreen));
            scene1.Children.Add(CreateRectangleTile(4, 2, Brushes.Gold));
            scene1.Children.Add(CreateRectangleTile(4, 3, Brushes.Indigo));

            scene1.Children.Add(CreateRectangleTile(6, 1, Brushes.Gainsboro, TileSize.ExtraLarge));

            scene1.Children.Add(CreateRectangleTile(11, 0, Brushes.Yellow));

            TileSceneContainer.Children.Add(scene1);

            var scene2 = new Canvas();
            scene2.Width = App.ViewWidth;
            scene2.Height = App.HeightContentArea;

			scene2.Children.Add(CreateRectangleTile(0, 0, Brushes.Firebrick, TileSize.Large));

			scene2.Children.Add(CreateRectangleTile(2, 1, Brushes.Gainsboro, TileSize.ExtraLarge));

			scene2.Children.Add(CreateRectangleTile(11, 0, Brushes.Yellow));

			TileSceneContainer.Children.Add(scene2);
        }

        private Rectangle CreateRectangleTile(int column, int row, SolidColorBrush brush, TileSize size = TileSize.Normal)
        {
            var position = GetCoordinatesForTileLocation(column, row);

            var tile = new Rectangle();
            switch (size)
            {
                case TileSize.Normal:
                    tile.Width = 130;
                    tile.Height = 130;
                    break;
                case TileSize.Large:
                    tile.Width = 284;
                    tile.Height = 284;
                    break;
                case TileSize.ExtraLarge:
                    tile.Width = 438;
                    tile.Height = 438;
                    break;
            }
            Canvas.SetLeft(tile, position.X);
            Canvas.SetTop(tile, position.Y);
            tile.Fill = brush;

            return tile;
        }

        private (int X, int Y) GetCoordinatesForTileLocation(int column, int row)
        {
            return ((column * 130) + ((column + 1) * 24) + 22, (row * 130) + ((row + 1) * 24) + 4);
        }
    }
}
