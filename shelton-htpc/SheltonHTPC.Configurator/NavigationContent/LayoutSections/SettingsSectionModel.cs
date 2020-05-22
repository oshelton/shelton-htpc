using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;
using SheltonHTPC.Dtos.Layout;
using SheltonHTPC.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFAspects.Core;
using WPFAspects.Utils;

namespace SheltonHTPC.NavigationContent.LayoutSections
{
    public sealed class SettingsSectionModel : NavigationSectionModelBase<LayoutContentModel>
    {
        public SettingsSectionModel(LayoutContentModel parent) : base(parent) { }

        public override string Title { get; } = "Settings";

        public override bool Scrollable => true;

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
        /// Tracker for changes to the Settings object being edited.
        /// </summary>
        public DirtyTracker SettingsTracker
        {
            get => CheckIsOnMainThread(_SettingsTracker);
            set => SetPropertyBackingValue(value, ref _SettingsTracker);
        }

        public override async Task Initialize(string dataPath)
        {
            LayoutSettingsDto dto = null;
            await Task.Run(() =>
            {
                dto = LayoutDataManager.Repo.FirstOrDefault<LayoutSettingsDto>(x => true, collectionName: LayoutSettingsDto.CollectionName);
                if (dto == null)
                {
                    dto = new LayoutSettingsDto
                    {
                        Id = Guid.NewGuid()
                    };
                    LayoutDataManager.Repo.Insert(dto);
                }
            });

            DispatcherHelper.InvokeIfNecessary(() =>
            {
                var viewModel = new EditableLayoutSettings(dto);
                _PersistedSettings = viewModel;
            });
        }

        protected override Task ActivateCore()
        {
            BeingEditedSettingsModel = _PersistedSettings.Duplicate();

            SettingsTracker = new DirtyTracker(BeingEditedSettingsModel);
            _IsDirtyUpdater = this.WhenAnyValue(x => x.SettingsTracker.IsDirty)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => IsDirty = x);

            return Task.CompletedTask;
        }

        protected override Task ParentNavigatesFromCore()
        {
            SettingsTracker.Dispose();
            _IsDirtyUpdater.Dispose();
            BeingEditedSettingsModel = null;

            return Task.CompletedTask;
        }

        public override Task OnSaved()
        {
            _PersistedSettings.MergeChangesFromOther(BeingEditedSettingsModel);

            var justEdited = BeingEditedSettingsModel.Duplicate();

            Parent.OngoingTaskManager.CreateAndStartOngoingTask("Updating Layout Info", OngoingTaskModel.ProgressDisplayKind.INDETERMINATE, taskModel =>
            {
                var dto = justEdited.CreateDto();
                LayoutDataManager.Repo.Update(dto, collectionName: LayoutSettingsDto.CollectionName);
            });

            SettingsTracker.SetInitialState();

            return Task.CompletedTask;
        }

        public override Task OnResetCore()
        {
            SettingsTracker.ResetToInitialState();

            return Task.CompletedTask;
        }

        private EditableLayoutSettings _PersistedSettings = null;
        private IDisposable _IsDirtyUpdater;
    }
}
