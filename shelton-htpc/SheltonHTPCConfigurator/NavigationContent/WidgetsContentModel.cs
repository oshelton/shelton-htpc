using SheltonHTPC.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFAspects.Core;

namespace SheltonHTPC.NavigationContent
{
    public class WidgetsContentModel : NavigationContentModelBase
    {
        public WidgetsContentModel(){ }

        public override bool CanNavigateAway() => true;

        public override Task Initialize(GeneralSettings generalSettings)
        {
            return Task.CompletedTask;
        }

        public override Task OnNavigatedTo()
        {
            return Task.CompletedTask;
        }

        public override Task OnNavigatedAwayFrom()
        {
            return Task.CompletedTask;
        }

        public override void OnSaved(object sender, RoutedEventArgs args)
        {
        }

        public override void OnReset(object sender, RoutedEventArgs args)
        {
        }

        public override ContentKind Kind => ContentKind.Widgets;
    }
}
