using LibVLCSharp.Shared;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VLCSharpExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LibVLC _LibVLC;
        MediaPlayer _MediaPlayer;

        public MainWindow()
        {
            this.DataContext = this;

            InitializeComponent();

            videoView.Loaded += VideoView_Loaded;
        }

        public bool HasDvdDrive => DriveInfo.GetDrives().Any(d => d.DriveType == DriveType.CDRom);

        private void VideoView_Loaded(object sender, RoutedEventArgs e)
        {
            Core.Initialize();

            _LibVLC = new LibVLC();
            _MediaPlayer = new MediaPlayer(_LibVLC);
            _MediaPlayer.PositionChanged += _MediaPlayer_PositionChanged;

            videoView.MediaPlayer = _MediaPlayer;
        }

        private void _MediaPlayer_PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                playbackText.Text = $"{TimeSpan.FromMilliseconds(e.Position * _MediaPlayer.Media.Duration).ToString(@"hh\:mm\:ss")} / {TimeSpan.FromMilliseconds(_MediaPlayer.Media.Duration).ToString(@"hh\:mm\:ss")}";
            });
        }

        private void DvdButton_Click(object sender, RoutedEventArgs e)
        {
            _MediaPlayer.Play(new Media(_LibVLC, $"dvd:///{DriveInfo.GetDrives().First(d => d.DriveType == DriveType.CDRom && d.IsReady).Name[0]}:/", FromType.FromLocation));
        }

        private void VideoButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.DefaultExt = ".mkv";
            dialog.Filter = "Video Files|*.m2v;*.m4v;*.avi;*.mpeg1;*.mpeg2;*.mts;*.divx;*.dv;*.flv;*.m1v;*.m2ts;*.mkv;*.mov;*.mpeg4;*.ts;*.vob;*.dat;*.bin;*.3g2;*.mpeg;*.mpg;*.3gp;*.wmv;*.asf";
            dialog.Title = "Pick a Video File";

            if (dialog.ShowDialog() ?? false)
                _MediaPlayer.Play(new Media(_LibVLC, dialog.FileName, FromType.FromPath));
        }

        private void AudioButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.DefaultExt = ".mp3";
            dialog.Filter = "Audio Files|*.dts;*.ogm;*.a52;*.aac;*.oma;*.spx;*.flac;*.m4a;*.mp1;*.ogg;*.wav;*.midi;*.xm;*.wma;*.ac3;*.mod;*.mp2;*.mp3;*.mka;*.m4p";
            dialog.Title = "Pick an Audio File";

            if (dialog.ShowDialog() ?? false)
                _MediaPlayer.Play(new Media(_LibVLC, dialog.FileName, FromType.FromPath));
        }
    }
}
