using SheltonHTPC.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.IO;
using WPFAspects.Utils;

namespace SheltonHTPC.NavigationContent.GeneralSettingsSections
{
    public partial class ToolsSection : ResourceDictionary
    {
        public ToolsSection()
        {
            InitializeComponent();
        }

        private async void PurgeButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var window = Window.GetWindow(button) as MetroWindow;
            var context = button.DataContext as GeneralSettings;
            
            var result = await window.ShowMessageAsync("Confirm Data Reset", "If you continue all Shelton HTPC data will be removed and you will not be able to get it back (except by restoring a previously made backup).  The application will also be restarted.\n\nAre you sure you want to continue?", MessageDialogStyle.AffirmativeAndNegative);

            if (result == MessageDialogResult.Affirmative)
                await WorkManager.StartApplicationBlockingWork(() => Utils.DataTools.PurgeData(context), "Purging data...");
        }
    }
}
