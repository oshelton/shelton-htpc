using MahApps.Metro.Controls;
using Pfz.AnimationManagement;
using Pfz.AnimationManagement.Wpf;
using ReactiveUI;
using SheltonHTPC.NavigationContent;
using System;
using System.Reactive.Linq;
using System.Windows;

namespace SheltonHTPC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            Pfz.AnimationManagement.Wpf.Initializer.Initialize();
            Model = new MainWindowViewModel();

            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _InitializedObserver = Model.ObservableForProperty(x => x.IsInitializing)
                .Where(change => !change.Value)
                .Subscribe(x => AnimateHidingLoadingIndicator());

            await Model.Initialize();

            Model.SetContent(ContentKind.GeneralSettings);
        }

        private void MetroWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            _InitializedObserver.Dispose();
        }

        private void AnimateHidingLoadingIndicator()
        {
            AnimationManager.Add(AnimationBuilder.BeginSequence()
                    .Range(1.0, 0.0, 1, (value) => LoadingContainer.Opacity = value)
                    .Add(() =>
                    {
                        LoadingContainer.Visibility = Visibility.Collapsed;
                        ContentPanel.IsEnabled = true;
                    })
                .EndSequence());
        }

        public MainWindowViewModel Model { get; private set; }

        private IDisposable _InitializedObserver;

        private void NavigationButton_Checked(object sender, RoutedEventArgs e)
        {
            Model.SetContent((ContentKind)((FrameworkElement)sender).Tag);
        }
    }
}
