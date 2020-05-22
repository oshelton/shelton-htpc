using SheltonHTPC.Data.Entities;
using SheltonHTPC.Dtos.Layout;
using SheltonHTPC.NavigationContent.LayoutSections;
using SheltonHTPC.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFAspects.Core;

namespace SheltonHTPC.NavigationContent
{
    public class LayoutContentModel : NavigationContentModelBase<LayoutContentModel>
    {
        public LayoutContentModel(OngoingTaskManager taskManager)
            : base(taskManager)
        {
            Sections = new ReadOnlyCollection<NavigationSectionModelBase<LayoutContentModel>>(new NavigationSectionModelBase<LayoutContentModel>[]
            {
                new SettingsSectionModel(this),
                new BackgroundSetsSectionModel(this)
            });
        }

        public override ContentKind Kind { get; } = ContentKind.Layout;

        public override ReadOnlyCollection<NavigationSectionModelBase<LayoutContentModel>> Sections { get; }

        public override Task Initialize(GeneralSettings generalSettings)
        {
            if (generalSettings is null)
                throw new ArgumentNullException(nameof(generalSettings));

            _GeneralSettings = generalSettings;

            string dataPath = generalSettings.DataPath;
            return Task.Run(() =>
            {
                LayoutDataManager.UpdateDataDirectory(dataPath);

                IEnumerable<Task> initTasks = null;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    initTasks = Sections.Select(s => s.Initialize(dataPath));
                });
                Task.WhenAll(initTasks).Wait();
            });
        }

        private GeneralSettings _GeneralSettings = null;
    }
}
