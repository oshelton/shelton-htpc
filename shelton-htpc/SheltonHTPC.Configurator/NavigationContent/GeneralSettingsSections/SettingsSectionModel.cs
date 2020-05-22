using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;
using SheltonHTPC.Dtos.Layout;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFAspects.Core;

namespace SheltonHTPC.NavigationContent.GeneralSettingsSections
{
    public sealed class SettingsSectionModel : NavigationSectionModelBase<GeneralSettingsContentModel>
    {
        public SettingsSectionModel(GeneralSettingsContentModel parent) : base(parent) { }

        public override string Title { get; } = "Settings";

        public override bool Scrollable => true;

        private DirtyTrackingGroup _SettingsTabTracker = null;
        /// <summary>
        /// Get whether or not there have been any changes made to the settings tab.
        /// </summary>
        public DirtyTrackingGroup SettingsTabTracker
        {
            get => CheckIsOnMainThread(_SettingsTabTracker);
            set => SetPropertyBackingValue(value, ref _SettingsTabTracker);
        }

        protected override Task ActivateCore()
        {
            SettingsTabTracker = Parent.SettingsTracker.CreateDirtyTrackingGroup("SettingsTab",
                nameof(Parent.BeingEditedSettingsModel.EnableMouse), nameof(Parent.BeingEditedSettingsModel.RunOnStartup), nameof(Parent.BeingEditedSettingsModel.IdleWaitMinutes));

            _IsDirtyUpdater = this.WhenAnyValue(x => x.SettingsTabTracker.IsDirty)
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
