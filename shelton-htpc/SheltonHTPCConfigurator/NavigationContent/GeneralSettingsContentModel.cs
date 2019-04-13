using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheltonHTPC.NavigationContent
{
    public class GeneralSettingsContentModel : NavigationContentModelBase
    {
        public override ContentKind Kind => ContentKind.GeneralSettings;

        public override bool CanNavigateAway() => true;

        public override Task Initialize()
        {
            return Task.CompletedTask;
        }
    }
}
