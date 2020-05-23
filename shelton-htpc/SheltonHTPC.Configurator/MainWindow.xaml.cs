using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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

        private async void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Model.CurrentContentModel.CanNavigateAway)
            {
                e.Cancel = true;
                await this.ShowMessageAsync("Unsaved Data Present", "There are unsaved changes in the application; you cannot close the application until you save or reset them.", MessageDialogStyle.Affirmative).ConfigureAwait(true);
            }
        }

        public MainWindowViewModel Model { get; private set; }

        private void NavigationButton_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Model.ChangeContentTo((ContentKind)((FrameworkElement)sender).Tag);
            e.Handled = true;
        }
    }
}
