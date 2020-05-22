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
    public class WebSitesContentModel : NavigationContentModelBase<WebSitesContentModel>
    {
        public WebSitesContentModel(OngoingTaskManager taskManager)
            : base(taskManager)
        {
            Sections = new ReadOnlyCollection<NavigationSectionModelBase<WebSitesContentModel>>(Array.Empty<NavigationSectionModelBase<WebSitesContentModel>>());
        }

        public override ContentKind Kind => ContentKind.WebSites;

        public override ReadOnlyCollection<NavigationSectionModelBase<WebSitesContentModel>> Sections { get; }
    }
}
