using DynamicData;
using SheltonHTPC.Data.Entities;
using SheltonHTPC.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFAspects.Core;
using WPFAspects.Utils;

namespace SheltonHTPC.NavigationContent
{
    public abstract class NavigationContentModelBase : Model
    {
        public NavigationContentModelBase(OngoingTaskManager taskManager)
        {
            OngoingTaskManager = taskManager;
        }

        public abstract ContentKind Kind { get; }

        public OngoingTaskManager OngoingTaskManager { get; }

        private bool _CanNavigateAway = false;
        /// <summary>
        /// Whether or not this tab can be navigated away from.
        /// </summary>
        public bool CanNavigateAway
        {
            get => CheckIsOnMainThread(_CanNavigateAway);
            set => SetPropertyBackingValue(value, ref _CanNavigateAway);
        }

        public abstract Task Initialize(GeneralSettings settings);
        public abstract Task OnNavigatedTo();
        public abstract Task OnNavigatedAwayFrom();
    }

    /// <summary>
    /// Base class for 
    /// </summary>
    public abstract class NavigationContentModelBase<T> : NavigationContentModelBase where T : NavigationContentModelBase<T>
    {
        public NavigationContentModelBase(OngoingTaskManager taskManager): base(taskManager) { }

        public abstract ReadOnlyCollection<NavigationSectionModelBase<T>> Sections { get; }

        private bool _IsSavingData = false;
        /// <summary>
        /// Whether or not data is currently being saved.
        /// </summary>
        public bool IsSavingData
        {
            get => CheckIsOnMainThread(_IsSavingData);
            private set => SetPropertyBackingValue(value, ref _IsSavingData);
        }

        private bool _IsResettingData = false;
        /// <summary>
        /// Whether or not data is currently being saved.
        /// </summary>
        public bool IsResettingData
        {
            get => CheckIsOnMainThread(_IsResettingData);
            private set => SetPropertyBackingValue(value, ref _IsResettingData);
        }

        private bool _IsDirty = false;
        /// <summary>
        /// Whether or not this tab has unsaved changes.
        /// </summary>
        public bool IsDirty
        {
            get => CheckIsOnMainThread(_IsDirty);
            set => SetPropertyBackingValue(value, ref _IsDirty);
        }

        public override Task Initialize(GeneralSettings generalSettings)
        {
            if (generalSettings is null)
                throw new ArgumentNullException(nameof(generalSettings));

            string dataPath = generalSettings.DataPath;
            return Task.Run(() =>
            {
                IEnumerable<Task> initTasks = null;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    initTasks = Sections.Select(s => s.Initialize(dataPath));
                });
                Task.WhenAll(initTasks).Wait();
            });
        }

        public override Task OnNavigatedTo()
        {
            _IsDirtyUpdater = Sections
                .AsObservableChangeSet()
                .AutoRefresh(x => x.IsDirty)
                .ToCollection()
                .Select(x => x.Any(y => y.IsDirty))
                .Subscribe(x =>
                {
                    IsDirty = x;
                    CanNavigateAway = !x;
                });

            return OnNavigatedToCore();
        }

        protected virtual Task OnNavigatedToCore() => Task.CompletedTask;

        public override Task OnNavigatedAwayFrom()
        {
             _IsDirtyUpdater.Dispose();

            return Task.WhenAll(Sections.Select(s => s.OnParentNavigatesFrom()))
                .ContinueWith(t => DispatcherHelper.InvokeIfNecessary(() => OnNavigatedAwayFromCore()));
        }

        protected virtual Task OnNavigatedAwayFromCore() => Task.CompletedTask;

        public virtual async void OnSelectedTabChanged(object sender, EventArgs args)
        {
            if (sender is null)
                throw new ArgumentNullException(nameof(sender));

            var tabControl = sender as TabControl;
            if (tabControl.SelectedItem is NavigationSectionModelBase<T>)
                await ((NavigationSectionModelBase<T>)tabControl.SelectedItem).OnActivated().ConfigureAwait(true);
        }

        public async void OnSaved(object sender, RoutedEventArgs args)
        {
            IsSavingData = true;

            await OnSavedCore(sender, args).ConfigureAwait(true);

            IsSavingData = false;
        }

        protected virtual Task OnSavedCore(object sender, RoutedEventArgs args) => Task.WhenAll(Sections.Where(s => s.IsDirty).Select(s => s.OnSaved()));

        public virtual async void OnReset(object sender, RoutedEventArgs args)
        {
            IsResettingData = true;

            await Task.WhenAll(Sections.Where(s => s.IsDirty).Select(s => s.OnReset())).ConfigureAwait(true);

            IsResettingData = false;
        }

        private IDisposable _IsDirtyUpdater;
    }
}
