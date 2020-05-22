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
    public class MoviesContentModel : NavigationContentModelBase<MoviesContentModel>
    {
        public MoviesContentModel(OngoingTaskManager taskManager)
            : base(taskManager)
        {
            Sections = new ReadOnlyCollection<NavigationSectionModelBase<MoviesContentModel>>(Array.Empty<NavigationSectionModelBase<MoviesContentModel>>());
        }

        public override ContentKind Kind => ContentKind.Movies;

        public override ReadOnlyCollection<NavigationSectionModelBase<MoviesContentModel>> Sections { get; }
    }
}
