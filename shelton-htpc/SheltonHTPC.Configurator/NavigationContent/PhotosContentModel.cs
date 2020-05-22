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
    public class PhotosContentModel : NavigationContentModelBase<PhotosContentModel>
    {
        public PhotosContentModel(OngoingTaskManager taskManager)
            : base(taskManager)
        {
            Sections = new ReadOnlyCollection<NavigationSectionModelBase<PhotosContentModel>>(Array.Empty<NavigationSectionModelBase<PhotosContentModel>>());
        }

        public override ContentKind Kind => ContentKind.Photos;

        public override ReadOnlyCollection<NavigationSectionModelBase<PhotosContentModel>> Sections { get; }
    }
}
