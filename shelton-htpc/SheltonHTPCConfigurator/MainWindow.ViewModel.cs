using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SheltonHTPC.Data.Entities;
using SheltonHTPC.NavigationContent;
using WPFAspects.Core;

namespace SheltonHTPC
{
    public class MainWindowViewModel : Model
    {
        public MainWindowViewModel()
        {
            _NavigationContentModels = new Dictionary<ContentKind, NavigationContentModelBase>()
            {
                { ContentKind.GeneralSettings, new GeneralSettingsContentModel() },
                { ContentKind.Layout, new LayoutContentModel() },
                { ContentKind.Movies, new MoviesContentModel() },
                { ContentKind.Series, new SeriesContentModel() },
                { ContentKind.Music, new MusicContentModel() },
                { ContentKind.Photos, new PhotosContentModel() },
                { ContentKind.Games, new GamesContentModel() },
                { ContentKind.Applications, new ApplicationsContentModel() },
                { ContentKind.WebSites, new WebSitesContentModel() },
                { ContentKind.Widgets, new WidgetsContentModel() },
                { ContentKind.WebAccess, new WebAccessContentModel() },
            };
        }

        public async Task Initialize()
        {
            IsInitializing = true;
            var result = await GeneralSettings.Deserialize();

            var initTasks = new List<Task>();
            foreach (var contentModel in _NavigationContentModels.Values)
                initTasks.Add(contentModel.Initialize(result));

            await Task.Delay(5000);

            await Task.WhenAll(initTasks.ToArray());

            GeneralSettings = result;
            IsInitializing = false;
        }

        public void ChangeContentTo(ContentKind kind)
        {
            NavigationContentModelBase newContent = null;
            if (_NavigationContentModels.TryGetValue(kind, out newContent))
            {
                if (CurrentContentModel != null && !CurrentContentModel.CanNavigateAway())
                {
                    //TODO: Confirm, clean up, etc...
                }
                else
                    CurrentContentModel = newContent;
            }
            else
                MessageBox.Show("Unhandled content kind :(", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                
        }

        private bool _IsInitializing = false;
        public bool IsInitializing
        {
            get => _IsInitializing;
            private set => SetPropertyBackingValue(value, ref _IsInitializing);
        }

        private GeneralSettings _GeneralSettings = null;
        public GeneralSettings GeneralSettings
        {
            get => _GeneralSettings;
            private set => SetPropertyBackingValue(value, ref _GeneralSettings);
        }

        private NavigationContentModelBase _CurrentContentModel = null;
        public NavigationContentModelBase CurrentContentModel
        {
            get => _CurrentContentModel;
            set
            {
                if (_CurrentContentModel != null && value != _CurrentContentModel)
                    _CurrentContentModel.OnNavigatedAwayFrom();

                if (SetPropertyBackingValue(value, ref _CurrentContentModel) && value != null)
                    _CurrentContentModel.OnNavigatedTo();
            }
        }

        private Dictionary<ContentKind, NavigationContentModelBase> _NavigationContentModels;
    }
}
