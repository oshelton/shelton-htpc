using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheltonHTPC.Data.Entities
{
    public class GeneralSettings: WPFAspects.Core.Model
    {
        /// <summary>
        /// Retrieve an instance of the GeneralSettings from the file system. 
        /// </summary>
        public static Task<GeneralSettings> Deserialize()
        {
            return Task.Run(() =>
            {
                string dataPathFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SheltonHTPC", "DataPath.txt");

                if (File.Exists(dataPathFilePath))
                {
                    string dataPathFileContents = File.ReadAllText(dataPathFilePath);

                    var results = new GeneralSettings();
                    string settingsFilePath = null;
                    //File should only contain the directory for the datapath.
                    if (!string.IsNullOrEmpty(dataPathFileContents))
                    {
                        settingsFilePath = Path.Combine(dataPathFileContents, "GeneralSettings.json");

                        // read JSON directly from a file
                        using (StreamReader file = File.OpenText(settingsFilePath))
                        using (JsonTextReader reader = new JsonTextReader(file))
                        {
                            var jsonObj = (JObject)JToken.ReadFrom(reader);

                            results.DataPath = dataPathFileContents;
                            results.RunOnStartup = (bool)jsonObj[nameof(RunOnStartup)];
                            results.IdleWaitMinutes = (uint)jsonObj[nameof(IdleWaitMinutes)];
                            results.EnableMovies = (bool)jsonObj[nameof(EnableMovies)];
                            results.EnableSeries = (bool)jsonObj[nameof(EnableSeries)];
                            results.EnableMusic = (bool)jsonObj[nameof(EnableMusic)];
                            results.EnablePhotos = (bool)jsonObj[nameof(EnablePhotos)];
                            results.EnableGames = (bool)jsonObj[nameof(EnableGames)];
                            results.EnableWebAccess = (bool)jsonObj[nameof(EnableWebAccess)];
                        }
                    }
                    else
                        results.DataPath = Path.GetDirectoryName(dataPathFilePath);

                    return results;
                }
                else
                {
                    var results = new GeneralSettings()
                    {
                        DataPath = Path.GetDirectoryName(dataPathFilePath),
                    };
                    return results;
                }
            });
        }

        /// <summary>
        /// Update the saved settings.
        /// </summary>
        public Task Serialize()
        {
            //Update data path.
            string dataPathFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SheltonHTPC", "DataPath.txt");
            File.WriteAllText(dataPathFilePath, DataPath);

            //Update the general settings.
            string settingsFilePath = Path.Combine(DataPath, "GeneralSettings.json");

            var jsonRoot = new JObject();
            jsonRoot[nameof(RunOnStartup)] = RunOnStartup;
            jsonRoot[nameof(IdleWaitMinutes)] = IdleWaitMinutes;
            jsonRoot[nameof(EnableMovies)] = EnableMovies;
            jsonRoot[nameof(EnableSeries)] = EnableSeries;
            jsonRoot[nameof(EnableMusic)] = EnableMusic;
            jsonRoot[nameof(EnablePhotos)] = EnablePhotos;
            jsonRoot[nameof(EnableGames)] = EnableGames;
            jsonRoot[nameof(EnableWebAccess)] = EnableWebAccess;

            using (StreamWriter file = File.CreateText(settingsFilePath))
            using (JsonTextWriter writer = new JsonTextWriter(file))
                return jsonRoot.WriteToAsync(writer);
        }

        private string _DataPath = null;
        /// <summary>
        /// The path to the directory that contains the assets, settings, and databases for the application.
        /// </summary>
        public string DataPath
        {
            get => _DataPath;
            set => SetPropertyBackingValue(value, ref _DataPath);
        }

        private bool _RunOnStartup = false;
        /// <summary>
        /// Whether or not the application should run on startup.
        /// </summary>
        public bool RunOnStartup
        {
            get => _RunOnStartup;
            set => SetPropertyBackingValue(value, ref _RunOnStartup);
        }

        private uint _IdleWaitMinutes = 15;
        /// <summary>
        /// Number of minutes before the application becomes idle.
        /// </summary>
        public uint IdleWaitMinutes
        {
            get => _IdleWaitMinutes;
            set => SetPropertyBackingValue(value, ref _IdleWaitMinutes);
        }

        #region Feature switches.
        private bool _EnableMovies = true;
        /// <summary>
        /// Whether or not Movies are enabled in the software.
        /// </summary>
        public bool EnableMovies
        {
            get => _EnableMovies;
            set => SetPropertyBackingValue(value, ref _EnableMovies);
        }

        private bool _EnableSeries = true;
        /// <summary>
        /// Whether or not TV Series support is enabled in the application.
        /// </summary>
        public bool EnableSeries
        {
            get => _EnableSeries;
            set => SetPropertyBackingValue(value, ref _EnableSeries);
        }

        private bool _EnableMusic = true;
        /// <summary>
        /// Whether or not Music support is enabled in the application.
        /// </summary>
        public bool EnableMusic
        {
            get => _EnableMusic;
            set => SetPropertyBackingValue(value, ref _EnableMusic);
        }

        private bool _EnablePhotos = false;
        /// <summary>
        /// Whether or not pictures is enabled in the application.
        /// </summary>
        public bool EnablePhotos
        {
            get => _EnablePhotos;
            set => SetPropertyBackingValue(value, ref _EnablePhotos);
        }

        private bool _EnableGames = true;
        /// <summary>
        /// Whether or not Games are supported in the application.
        /// </summary>
        public bool EnableGames
        {
            get => _EnableGames;
            set => SetPropertyBackingValue(value, ref _EnableGames);
        }

        private bool _EnableWebAccess = true;
        /// <summary>
        /// Whether or not web access is supported by the application.
        /// </summary>
        public bool EnableWebAccess
        {
            get => _EnableWebAccess;
            set => SetPropertyBackingValue(value, ref _EnableWebAccess);
        }
        #endregion

        public GeneralSettings Duplicate()
        {
            return new GeneralSettings()
            {
                _DataPath = this.DataPath,
                _RunOnStartup = this.RunOnStartup,
                _IdleWaitMinutes = this.IdleWaitMinutes,
                _EnableMovies = this.EnableMovies,
                _EnableSeries = this.EnableSeries,
                _EnableMusic = this.EnableMusic,
                _EnablePhotos = this.EnablePhotos,
                _EnableGames = this.EnableGames,
                _EnableWebAccess = this.EnableWebAccess,
            };
        }
    }
}
