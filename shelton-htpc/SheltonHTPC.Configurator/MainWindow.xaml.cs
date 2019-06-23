using MahApps.Metro.Controls;
using ReactiveUI;
using SheltonHTPC.NavigationContent;
using System;
using System.Reactive.Linq;
using System.Windows;
using WPFAspects.Utils;

namespace SheltonHTPC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            Model = new MainWindowViewModel();
            this.DataContext = Model;

            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await WorkManager.StartApplicationBlockingWork(Model.Initialize, "Loading...");

            Model.ChangeContentTo(ContentKind.GeneralSettings);
        }

        private void MetroWindow_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        public MainWindowViewModel Model { get; private set; }

        private void NavigationButton_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Model.ChangeContentTo((ContentKind)((FrameworkElement)sender).Tag);
            e.Handled = true;
        }
    }
}
