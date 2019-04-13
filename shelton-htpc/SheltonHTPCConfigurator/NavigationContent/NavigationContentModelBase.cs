using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFAspects.Core;

namespace SheltonHTPC.NavigationContent
{
    public abstract class NavigationContentModelBase : Model
    {
        public abstract ContentKind Kind { get; }

        public abstract bool CanNavigateAway();

        public abstract Task Initialize();
    }
}
