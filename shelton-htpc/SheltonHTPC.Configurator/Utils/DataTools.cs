using SheltonHTPC.Data;
using SheltonHTPC.Data.Entities;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace SheltonHTPC.Utils
{
    public static class DataTools
    {
        public static async Task PurgeData(GeneralSettings settings)
        {
            await Task.Run(() =>
            {
                Directory.Delete(settings.DataPath, true);

                if (File.Exists(DataHelper.DataPathFilePath))
                    File.Delete(DataHelper.DataPathFilePath);
            });

            Application.Current.MainWindow.Hide();
            System.Windows.Forms.Application.Restart();
            Environment.Exit(0);
        }
    }
}
