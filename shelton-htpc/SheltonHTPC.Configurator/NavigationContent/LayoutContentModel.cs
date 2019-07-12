using SheltonHTPC.Data.Entities;
using SheltonHTPC.Dtos.Layout;
using SheltonHTPC.NavigationContent.LayoutSections;
using SheltonHTPC.Utils;
using System;
using System.Threading.Tasks;
using System.Windows;
using WPFAspects.Core;

namespace SheltonHTPC.NavigationContent
{
    public class LayoutContentModel : NavigationContentModelBase
    {
        public LayoutContentModel(OngoingTaskManager taskManager)
            : base(taskManager) { }

        public override bool CanNavigateAway() => true;

        public override Task Initialize(GeneralSettings generalSettings)
        {
            _GeneralSettings = generalSettings;

            string dataPath = generalSettings.DataPath;
            return Task.Run(() =>
            {
                LayoutDataManager.UpdateDataDirectory(dataPath);
                var dto = LayoutDataManager.Repo.FirstOrDefault<LayoutSettingsDto>(collectionName: LayoutSettingsDto.CollectionName);
                if (dto == null)
                {
                    dto = new LayoutSettingsDto
                    {
                        Id = Guid.NewGuid()
                    };
                    LayoutDataManager.Repo.Insert(dto);
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var viewModel = new EditableLayoutSettings(dto);
                    _PersistedSettings = viewModel;
                });
            });
        }

        public override Task OnNavigatedTo()
        {
            BeingEditedSettingsModel = _PersistedSettings.Duplicate();

            SettingsTracker = new DirtyTracker(BeingEditedSettingsModel);

            return Task.CompletedTask;
        }

        public override Task OnNavigatedAwayFrom()
        {
            SettingsTracker.Dispose();
            BeingEditedSettingsModel = null;

            return Task.CompletedTask;
        }

        public override async void OnSaved(object sender, RoutedEventArgs args)
        {
            _PersistedSettings.MergeChangesFromOther(BeingEditedSettingsModel);

            IsSavingData = true;
            var justEdited = BeingEditedSettingsModel.Duplicate();

            await OngoingTaskManager.CreateAndStartOngoingTask("Updating Layout Info", OngoingTaskModel.ProgressDisplayKind.INDETERMINATE, taskModel =>
            {
                var dto = justEdited.CreateDto();
                LayoutDataManager.Repo.Update(dto, collectionName: LayoutSettingsDto.CollectionName);
            }).Task;

            SettingsTracker.SetInitialState();
            IsSavingData = false;
        }

        public override void OnReset(object sender, RoutedEventArgs args)
        {
            SettingsTracker.ResetToInitialState();
        }

        public override ContentKind Kind => ContentKind.Layout;

        private EditableLayoutSettings _BeingEditedSettingsModel;
        /// <summary>
        /// The GeneralSettings model being edited currently.
        /// </summary>
        public EditableLayoutSettings BeingEditedSettingsModel
        {
            get => CheckIsOnMainThread(_BeingEditedSettingsModel);
            set => SetPropertyBackingValue(value, ref _BeingEditedSettingsModel);
        }

        private DirtyTracker _SettingsTracker;
        /// <summary>
        /// Tracker for changes to the General Settings object being edited.
        /// </summary>
        public DirtyTracker SettingsTracker
        {
            get => CheckIsOnMainThread(_SettingsTracker);
            set => SetPropertyBackingValue(value, ref _SettingsTracker);
        }

        private bool _IsSavingData = false;
        /// <summary>
        /// Whether or not data is currently being saved.
        /// </summary>
        public bool IsSavingData
        {
            get => CheckIsOnMainThread(_IsSavingData);
            set => SetPropertyBackingValue(value, ref _IsSavingData);
        }

        private GeneralSettings _GeneralSettings = null;
        private EditableLayoutSettings _PersistedSettings = null;
    }
}
