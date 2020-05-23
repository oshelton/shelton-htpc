using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SheltonHTPC.Data.Entities;
using SheltonHTPC.NavigationContent;
using SheltonHTPC.Utils;
using WPFAspects.Core;

namespace SheltonHTPC
{
    public class MainWindowViewModel : Model
    {
        public MainWindowViewModel()
        {
            //Build collection of associations between content kinds and their view models.
            _NavigationContentModels = new Dictionary<ContentKind, NavigationContentModelBase>()
            {
                { ContentKind.GeneralSettings, new GeneralSettingsContentModel(OngoingTaskManager) },
                { ContentKind.Layout, new LayoutContentModel(OngoingTaskManager) },
                { ContentKind.Movies, new MoviesContentModel(OngoingTaskManager) },
                { ContentKind.Series, new SeriesContentModel(OngoingTaskManager) },
                { ContentKind.Music, new MusicContentModel(OngoingTaskManager) },
                { ContentKind.Photos, new PhotosContentModel(OngoingTaskManager) },
                { ContentKind.Games, new GamesContentModel(OngoingTaskManager) },
                { ContentKind.Applications, new ApplicationsContentModel(OngoingTaskManager) },
                { ContentKind.WebSites, new WebSitesContentModel(OngoingTaskManager) },
                { ContentKind.Widgets, new WidgetsContentModel(OngoingTaskManager) },
                { ContentKind.WebAccess, new WebAccessContentModel(OngoingTaskManager) },
            };
        }

        /// <summary>
        /// Perform the startup initialization.
        /// </summary>
        public async Task Initialize()
        {
            IsInitializing = true;
            var result = await GeneralSettings.Deserialize().ConfigureAwait(true);

            var initTasks = new List<Task>();
            foreach (var contentModel in _NavigationContentModels.Values)
                initTasks.Add(contentModel.Initialize(result));

            await Task.WhenAll(initTasks.ToArray()).ConfigureAwait(true);

            GeneralSettings = result;
            IsInitializing = false;
        }

        /// <summary>
        /// Change the current main content of the application to another kind of content.
        /// </summary>
        public void ChangeContentTo(ContentKind kind)
        {
            NavigationContentModelBase newContent = null;
            if (_NavigationContentModels.TryGetValue(kind, out newContent))
            {
                if (CurrentContentModel == null || CurrentContentModel.CanNavigateAway)
                    CurrentContentModel = newContent;
            }
            else
                MessageBox.Show("Unhandled content kind :(", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                
        }

        private bool _IsInitializing = false;
        /// <summary>
        /// Whether or not the application is in its initial startup/initialization phase.
        /// </summary>
        public bool IsInitializing
        {
            get => CheckIsOnMainThread(_IsInitializing);
            private set => SetPropertyBackingValue(value, ref _IsInitializing);
        }

        private GeneralSettings _GeneralSettings = null;
        /// <summary>
        /// General application settings used by the application.
        /// </summary>
        public GeneralSettings GeneralSettings
        {
            get => CheckIsOnMainThread(_GeneralSettings);
            private set => SetPropertyBackingValue(value, ref _GeneralSettings);
        }

        private NavigationContentModelBase _CurrentContentModel = null;
        /// <summary>
        /// Currently selected navigation model.
        /// </summary>
        public NavigationContentModelBase CurrentContentModel
        {
            get => CheckIsOnMainThread(_CurrentContentModel);
            set
            {
                Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        if (_CurrentContentModel != null && value != _CurrentContentModel)
                            await _CurrentContentModel.OnNavigatedAwayFrom().ConfigureAwait(true);

                        if (SetPropertyBackingValue(value, ref _CurrentContentModel) && value != null)
                            await _CurrentContentModel.OnNavigatedTo().ConfigureAwait(true);
                    });
                });
            }
        }

        /// <summary>
        /// Object for tracking ongoing tasks.
        /// </summary>
        public OngoingTaskManager OngoingTaskManager { get; } = new OngoingTaskManager();

        /// <summary>
        /// Collection of available content models.
        /// </summary>
        private Dictionary<ContentKind, NavigationContentModelBase> _NavigationContentModels;
    }
}
