using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WPFAspects.Core;

namespace SheltonHTPC.NavigationContent.GeneralSettingsSections
{
    public sealed class FeaturesSectionModel : NavigationSectionModelBase<GeneralSettingsContentModel>
    {
        public FeaturesSectionModel(GeneralSettingsContentModel parent) : base(parent) { }

        public override string Title { get; } = "Features";

        public override bool Scrollable => false;

        private DirtyTrackingGroup _FeaturesTabTracker = null;
        /// <summary>
        /// Get whether or not there have been any changes made to the features tab.
        /// </summary>
        public DirtyTrackingGroup FeaturesTabTracker
        {
            get => CheckIsOnMainThread(_FeaturesTabTracker);
            set => SetPropertyBackingValue(value, ref _FeaturesTabTracker);
        }

        protected override Task ActivateCore()
        {
            FeaturesTabTracker = Parent.SettingsTracker.CreateDirtyTrackingGroup("FeaturesTab",
                nameof(Parent.BeingEditedSettingsModel.EnableMovies), nameof(Parent.BeingEditedSettingsModel.EnableSeries), nameof(Parent.BeingEditedSettingsModel.EnableMusic), nameof(Parent.BeingEditedSettingsModel.EnablePhotos),
                nameof(Parent.BeingEditedSettingsModel.EnableGames), nameof(Parent.BeingEditedSettingsModel.EnableWebAccess), nameof(Parent.BeingEditedSettingsModel.EnableApplications), nameof(Parent.BeingEditedSettingsModel.EnableWebSites),
                nameof(Parent.BeingEditedSettingsModel.EnableWidgets));

            _IsDirtyUpdater = this.WhenAnyValue(x => x.FeaturesTabTracker.IsDirty)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => IsDirty = x);

            return Task.CompletedTask;
        }

        protected override Task ParentNavigatesFromCore()
        {
            _IsDirtyUpdater.Dispose();

            return Task.CompletedTask;
        }

        private IDisposable _IsDirtyUpdater;
    }
}
