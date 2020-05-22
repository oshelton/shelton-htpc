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
    public class MusicContentModel : NavigationContentModelBase<MusicContentModel>
    {
        public MusicContentModel(OngoingTaskManager taskManager)
            : base(taskManager)
        {
            Sections = new ReadOnlyCollection<NavigationSectionModelBase<MusicContentModel>>(Array.Empty<NavigationSectionModelBase<MusicContentModel>>());
        }

        public override ContentKind Kind => ContentKind.Music;

        public override ReadOnlyCollection<NavigationSectionModelBase<MusicContentModel>> Sections { get; }
    }
}
