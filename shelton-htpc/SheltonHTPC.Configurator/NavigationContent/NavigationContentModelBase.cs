using SheltonHTPC.Data.Entities;
using SheltonHTPC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFAspects.Core;

namespace SheltonHTPC.NavigationContent
{
    /// <summary>
    /// Base class for 
    /// </summary>
    public abstract class NavigationContentModelBase : Model
    {
        public NavigationContentModelBase(OngoingTaskManager taskManager)
        {
            OngoingTaskManager = taskManager;
        }

        public abstract bool CanNavigateAway();

        public abstract Task Initialize(GeneralSettings generalSettings);

        public abstract Task OnNavigatedTo();
        public abstract Task OnNavigatedAwayFrom();
        public abstract void OnSaved(object sender, RoutedEventArgs args);
        public abstract void OnReset(object sender, RoutedEventArgs args);

        protected OngoingTaskManager OngoingTaskManager { get; }

        public abstract ContentKind Kind { get; }
    }
}
