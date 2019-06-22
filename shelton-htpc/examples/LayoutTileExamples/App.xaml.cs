using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace LayoutTileExamples
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static double ViewWidth => 1920;
        public static double ViewHeight => 1080;

        public static double TopOfContentArea => 216;
        public static double HeightContentArea => 648;

        public static int TitleTextSize => 32;
        public static int SubtitleTextSize => 24;
        public static int NormalTextSize => 20;
    }
}
