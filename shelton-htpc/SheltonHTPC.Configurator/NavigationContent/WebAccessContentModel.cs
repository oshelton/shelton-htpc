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
    public class WebAccessContentModel : NavigationContentModelBase<WebAccessContentModel>
    {
        public WebAccessContentModel(OngoingTaskManager taskManager)
            : base(taskManager)
        {
            Sections = new ReadOnlyCollection<NavigationSectionModelBase<WebAccessContentModel>>(Array.Empty<NavigationSectionModelBase<WebAccessContentModel>>());
        }

        public override ContentKind Kind => ContentKind.WebAccess;

        public override ReadOnlyCollection<NavigationSectionModelBase<WebAccessContentModel>> Sections { get; }
    }
}
