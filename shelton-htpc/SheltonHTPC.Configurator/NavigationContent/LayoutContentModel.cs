using SheltonHTPC.Data.Entities;
using SheltonHTPC.Dtos.Layout;
using SheltonHTPC.NavigationContent.LayoutSections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFAspects.Core;

namespace SheltonHTPC.NavigationContent
{
    public class LayoutContentModel : NavigationContentModelBase
    {
        public LayoutContentModel(){ }

        public override bool CanNavigateAway() => true;

        public override Task Initialize(GeneralSettings generalSettings)
        {
            _GeneralSettings = generalSettings;

            return Task.Run(() =>
            {
                LayoutDataManager.UpdateDataDirectory(generalSettings.DataPath);
                var dto = LayoutDataManager.Repo.FirstOrDefault<LayoutSettingsDto>(collectionName: LayoutSettingsDto.CollectionName);
                if (dto == null)
                {
                    dto = new LayoutSettingsDto
                    {
                        Id = new Guid()
                    };
                    LayoutDataManager.Repo.Insert(dto);
                }

                var viewModel = new EditableLayoutSettings(dto);
                _PersistedSettings = viewModel;
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

        public override void OnSaved(object sender, RoutedEventArgs args)
        {
            _PersistedSettings.MergeChangesFromOther(BeingEditedSettingsModel);

            var justEdited = BeingEditedSettingsModel.Duplicate();
            Task.Run(() =>
            {
                var dto = justEdited.CreateDto();
                LayoutDataManager.Repo.Update(dto, collectionName: LayoutSettingsDto.CollectionName);
            });

            SettingsTracker.SetInitialState();
        }

        public override void OnReset(object sender, RoutedEventArgs args)
        {
        }

        public override ContentKind Kind => ContentKind.Layout;

        private EditableLayoutSettings _BeingEditedSettingsModel;
        /// <summary>
        /// The GeneralSettings model being edited currently.
        /// </summary>
        public EditableLayoutSettings BeingEditedSettingsModel
        {
            get => _BeingEditedSettingsModel;
            set => SetPropertyBackingValue(value, ref _BeingEditedSettingsModel);
        }

        private DirtyTracker _SettingsTracker;
        /// <summary>
        /// Tracker for changes to the General Settings object being edited.
        /// </summary>
        public DirtyTracker SettingsTracker
        {
            get => _SettingsTracker;
            set => SetPropertyBackingValue(value, ref _SettingsTracker);
        }

        private GeneralSettings _GeneralSettings = null;
        private EditableLayoutSettings _PersistedSettings = null;
    }
}
