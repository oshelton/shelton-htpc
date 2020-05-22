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
    public class SeriesContentModel : NavigationContentModelBase<SeriesContentModel>
    {
        public SeriesContentModel(OngoingTaskManager taskManager)
            : base(taskManager)
        {
            Sections = new ReadOnlyCollection<NavigationSectionModelBase<SeriesContentModel>>(Array.Empty<NavigationSectionModelBase<SeriesContentModel>>());
        }

        public override ContentKind Kind => ContentKind.Series;

        public override ReadOnlyCollection<NavigationSectionModelBase<SeriesContentModel>> Sections { get; }
    }
}
