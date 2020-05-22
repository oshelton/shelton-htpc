using SheltonHTPC.Data.Entities;
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
    public class WidgetsContentModel : NavigationContentModelBase<WidgetsContentModel>
    {
        public WidgetsContentModel(OngoingTaskManager taskManager)
            : base(taskManager)
        {
            Sections = new ReadOnlyCollection<NavigationSectionModelBase<WidgetsContentModel>>(Array.Empty<NavigationSectionModelBase<WidgetsContentModel>>());
        }

        public override ContentKind Kind => ContentKind.Widgets;

        public override ReadOnlyCollection<NavigationSectionModelBase<WidgetsContentModel>> Sections { get; }
    }
}
