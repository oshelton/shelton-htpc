using SheltonHTPC.Data.Entities;
using SheltonHTPC.NavigationContent.GeneralSettingsSections;
using SheltonHTPC.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFAspects.Core;

namespace SheltonHTPC.NavigationContent
{
    public class GeneralSettingsContentModel : NavigationContentModelBase<GeneralSettingsContentModel>
    {
        public GeneralSettingsContentModel(OngoingTaskManager taskManager)
            : base(taskManager)
        {
            Sections = new ReadOnlyCollection<NavigationSectionModelBase<GeneralSettingsContentModel>>(new NavigationSectionModelBase<GeneralSettingsContentModel>[]
            {
                new HomeSectionModel(this),
                new SettingsSectionModel(this),
                new FeaturesSectionModel(this),
                new ToolsSectionModel(this),
                new InputSectionModel(this)
            });
        }

        public override ContentKind Kind => ContentKind.GeneralSettings;

        public override ReadOnlyCollection<NavigationSectionModelBase<GeneralSettingsContentModel>> Sections { get; }

        private DirtyTracker _SettingsTracker;
        /// <summary>
        /// Tracker for changes to the General Settings object being edited.
        /// </summary>
        public DirtyTracker SettingsTracker
        {
            get => CheckIsOnMainThread(_SettingsTracker);
            set => SetPropertyBackingValue(value, ref _SettingsTracker);
        }

        private GeneralSettings _BeingEditedSettingsModel;
        /// <summary>
        /// The GeneralSettings model being edited currently.
        /// </summary>
        public GeneralSettings BeingEditedSettingsModel
        {
            get => CheckIsOnMainThread(_BeingEditedSettingsModel);
            set => SetPropertyBackingValue(value, ref _BeingEditedSettingsModel);
        }

        public override Task Initialize(GeneralSettings generalSettings)
        {
            _PersistedGeneralSettings = generalSettings;

            return base.Initialize(generalSettings);
        }

        protected override Task OnNavigatedToCore()
        {
            BeingEditedSettingsModel = _PersistedGeneralSettings.Duplicate();

            SettingsTracker = new DirtyTracker(BeingEditedSettingsModel);

            return Task.CompletedTask;
        }

        protected override Task OnNavigatedAwayFromCore()
        {
            SettingsTracker.Dispose();
            BeingEditedSettingsModel = null;

            return Task.CompletedTask;
        }

        protected override async Task OnSavedCore(object sender, RoutedEventArgs args)
        {
            if (SettingsTracker.IsDirty)
            {
                _PersistedGeneralSettings.MergeChangesFromOther(BeingEditedSettingsModel);

                var justEdited = BeingEditedSettingsModel.Duplicate();

                OngoingTaskManager.CreateAndStartOngoingTask("Saving General Information", OngoingTaskModel.ProgressDisplayKind.INDETERMINATE, taskModel =>
                {
                    justEdited.Serialize();
                });

                SettingsTracker.SetInitialState();
            }

            await base.OnSavedCore(sender, args).ConfigureAwait(true);
        }

        public override void OnReset(object sender, RoutedEventArgs args)
        {
            SettingsTracker.ResetToInitialState();

            base.OnReset(sender, args);
        }

        private GeneralSettings _PersistedGeneralSettings = null;
    }
}
