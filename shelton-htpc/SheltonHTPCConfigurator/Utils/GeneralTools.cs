using SheltonHTPC.Data;
using SheltonHTPC.Data.Entities;
using System;
using System.IO;
using System.Windows;

namespace SheltonHTPC.Utils
{
    public static class GeneralTools
    {
        public static void PurgeData(GeneralSettings settings)
        {
            Directory.Delete(settings.DataPath, true);

            if (File.Exists(DataHelper.DataPathFilePath))
                File.Delete(DataHelper.DataPathFilePath);

            Application.Current.MainWindow.Hide();
            System.Windows.Forms.Application.Restart();
            Environment.Exit(0);
        }
    }
}
