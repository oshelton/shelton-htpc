using SheltonHTPC.Data;
using SheltonHTPC.Data.Entities;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SheltonHTPC.Utils
{
    public static class DataTools
    {
        public static async Task PurgeData(GeneralSettings settings)
        {
            if (settings is null)
                throw new ArgumentNullException(nameof(settings));

            string dataPath = settings.DataPath;

            await Task.Run(() =>
            {
                int retries = 0;
                while (Directory.Exists(dataPath) && retries < 5)
                {
                    try
                    {
                        Directory.Delete(dataPath, true);
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(5000);
                        ++retries;
                    }
                }

                if (File.Exists(DataHelper.DataPathFilePath))
                    File.Delete(DataHelper.DataPathFilePath);
            });

            Application.Current.MainWindow.Hide();
            System.Windows.Forms.Application.Restart();
            Environment.Exit(0);
        }
    }
}
