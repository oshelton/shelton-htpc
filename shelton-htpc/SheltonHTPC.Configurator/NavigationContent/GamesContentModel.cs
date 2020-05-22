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
    public class GamesContentModel : NavigationContentModelBase<GamesContentModel>
    {
        public GamesContentModel(OngoingTaskManager taskManager)
            : base(taskManager)
        {
            Sections = new ReadOnlyCollection<NavigationSectionModelBase<GamesContentModel>>(Array.Empty<NavigationSectionModelBase<GamesContentModel>>());
        }

        public override ContentKind Kind => ContentKind.Games;

        public override ReadOnlyCollection<NavigationSectionModelBase<GamesContentModel>> Sections { get; }
    }
}
