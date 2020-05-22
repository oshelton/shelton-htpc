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
    public class ApplicationsContentModel : NavigationContentModelBase<ApplicationsContentModel>
    {
        public ApplicationsContentModel(OngoingTaskManager taskManager) 
            : base(taskManager)
        {
            Sections = new ReadOnlyCollection<NavigationSectionModelBase<ApplicationsContentModel>>(Array.Empty<NavigationSectionModelBase<ApplicationsContentModel>>());
        }

        public override ContentKind Kind => ContentKind.Applications;

        public override ReadOnlyCollection<NavigationSectionModelBase<ApplicationsContentModel>> Sections { get; }
    }
}
