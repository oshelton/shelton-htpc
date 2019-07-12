using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SheltonHTPC.Common.Utils;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace SheltonHTPC.Data.Entities
{
    /// <summary>
    /// Model for the general/non-specific settings for the application.
    /// </summary>
    public class GeneralSettings: WPFAspects.Core.Model, Interfaces.IGeneralSettings
    {
        /// <summary>
        /// Retrieve an instance of the GeneralSettings from the file system. 
        /// </summary>
        public static Task<GeneralSettings> Deserialize()
        {
            return Task.Run(() =>
            {
                string dataPathFilePath = DataHelper.DataPathFilePath;

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

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                results.DataPath = dataPathFileContents;
                                results.RunOnStartup = jsonObj.GetValueOrDefault(nameof(RunOnStartup), false);
                                results.IdleWaitMinutes = jsonObj.GetValueOrDefault(nameof(IdleWaitMinutes), 15u);
                                results.EnableMouse = jsonObj.GetValueOrDefault(nameof(EnableMouse), true);
                                results.EnableMovies = jsonObj.GetValueOrDefault(nameof(EnableMovies), true);
                                results.EnableSeries = jsonObj.GetValueOrDefault(nameof(EnableSeries), true);
                                results.EnableMusic = jsonObj.GetValueOrDefault(nameof(EnableMusic), true);
                                results.EnablePhotos = jsonObj.GetValueOrDefault(nameof(EnablePhotos), true);
                                results.EnableGames = jsonObj.GetValueOrDefault(nameof(EnableGames), true);
                                results.EnableWebAccess = jsonObj.GetValueOrDefault(nameof(EnableWebAccess), true);
                                results.EnableApplications = jsonObj.GetValueOrDefault(nameof(EnableApplications), true);
                                results.EnableWebSites = jsonObj.GetValueOrDefault(nameof(EnableWebSites), true);
                                results.EnableWidgets = jsonObj.GetValueOrDefault(nameof(EnableWidgets), true);
                            });
                        }
                    }
                    else
                        Application.Current.Dispatcher.Invoke(() => results.DataPath = Path.GetDirectoryName(dataPathFilePath));

                    return results;
                }
                else
                {
                    var results = new GeneralSettings()
                    {
                        _DataPath = Path.GetDirectoryName(dataPathFilePath),
                    };
                    results.Serialize().Wait();
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
            string dataPathFilePath = DataHelper.DataPathFilePath;
            if (!File.Exists(dataPathFilePath))
                Directory.CreateDirectory(Path.GetDirectoryName(dataPathFilePath));
            File.WriteAllText(dataPathFilePath, _DataPath);

            //Update the general settings.
            string settingsFilePath = Path.Combine(_DataPath, "GeneralSettings.json");

            var jsonRoot = new JObject();
            jsonRoot[nameof(RunOnStartup)] = _RunOnStartup;
            jsonRoot[nameof(IdleWaitMinutes)] = _IdleWaitMinutes;
            jsonRoot[nameof(EnableMouse)] = _EnableMouse;
            jsonRoot[nameof(EnableMovies)] = _EnableMovies;
            jsonRoot[nameof(EnableSeries)] = _EnableSeries;
            jsonRoot[nameof(EnableMusic)] = _EnableMusic;
            jsonRoot[nameof(EnablePhotos)] = _EnablePhotos;
            jsonRoot[nameof(EnableGames)] = _EnableGames;
            jsonRoot[nameof(EnableWebAccess)] = _EnableWebAccess;
            jsonRoot[nameof(EnableApplications)] = _EnableApplications;
            jsonRoot[nameof(EnableWebSites)] = _EnableWebSites;
            jsonRoot[nameof(EnableWidgets)] = _EnableWidgets;

            using (StreamWriter file = File.CreateText(settingsFilePath))
            using (JsonTextWriter writer = new JsonTextWriter(file))
                return jsonRoot.WriteToAsync(writer);
        }

        public GeneralSettings Duplicate()
        {
            return new GeneralSettings()
            {
                _DataPath = this.DataPath,
                _RunOnStartup = this.RunOnStartup,
                _IdleWaitMinutes = this.IdleWaitMinutes,
                _EnableMouse = this.EnableMouse,
                _EnableMovies = this.EnableMovies,
                _EnableSeries = this.EnableSeries,
                _EnableMusic = this.EnableMusic,
                _EnablePhotos = this.EnablePhotos,
                _EnableGames = this.EnableGames,
                _EnableWebAccess = this.EnableWebAccess,
                _EnableApplications = this.EnableApplications,
                _EnableWebSites = this.EnableWebSites,
                _EnableWidgets = this.EnableWidgets,
            };
        }

        public void MergeChangesFromOther(GeneralSettings other)
        {
            _DataPath = other.DataPath;
            _RunOnStartup = other.RunOnStartup;
            _IdleWaitMinutes = other.IdleWaitMinutes;
            _EnableMouse = other.EnableMouse;
            _EnableMovies = other.EnableMovies;
            _EnableSeries = other.EnableSeries;
            _EnableMusic = other.EnableMusic;
            _EnablePhotos = other.EnablePhotos;
            _EnableGames = other.EnableGames;
            _EnableWebAccess = other.EnableWebAccess;
            _EnableApplications = other.EnableApplications;
            _EnableWebSites = other.EnableWebSites;
            _EnableWidgets = other.EnableWidgets;

            RaisePropertyChanged(null);
        }

        private string _DataPath = null;
        /// <summary>
        /// The path to the directory that contains the assets, settings, and databases for the application.
        /// </summary>
        public string DataPath
        {
            get => CheckIsOnMainThread(_DataPath);
            set => SetPropertyBackingValue(value, ref _DataPath);
        }

        private bool _RunOnStartup = false;
        /// <summary>
        /// Whether or not the application should run on startup.
        /// </summary>
        public bool RunOnStartup
        {
            get => CheckIsOnMainThread(_RunOnStartup);
            set => SetPropertyBackingValue(value, ref _RunOnStartup);
        }

        private uint _IdleWaitMinutes = 15;
        /// <summary>
        /// Number of minutes before the application becomes idle.
        /// </summary>
        public uint IdleWaitMinutes
        {
            get => CheckIsOnMainThread(_IdleWaitMinutes);
            set => SetPropertyBackingValue(value, ref _IdleWaitMinutes);
        }

        private bool _EnableMouse = true;
        /// <summary>
        /// Whether or not the mouse should be enabled.
        /// </summary>
        public bool EnableMouse
        {
            get => CheckIsOnMainThread(_EnableMouse);
            set => SetPropertyBackingValue(value, ref _EnableMouse);
        }

        #region Feature switches.
        private bool _EnableMovies = true;
        /// <summary>
        /// Whether or not Movies are enabled in the software.
        /// </summary>
        public bool EnableMovies
        {
            get => CheckIsOnMainThread(_EnableMovies);
            set => SetPropertyBackingValue(value, ref _EnableMovies);
        }

        private bool _EnableSeries = true;
        /// <summary>
        /// Whether or not TV Series support is enabled in the application.
        /// </summary>
        public bool EnableSeries
        {
            get => CheckIsOnMainThread(_EnableSeries);
            set => SetPropertyBackingValue(value, ref _EnableSeries);
        }

        private bool _EnableMusic = true;
        /// <summary>
        /// Whether or not Music support is enabled in the application.
        /// </summary>
        public bool EnableMusic
        {
            get => CheckIsOnMainThread(_EnableMusic);
            set => SetPropertyBackingValue(value, ref _EnableMusic);
        }

        private bool _EnablePhotos = true;
        /// <summary>
        /// Whether or not pictures is enabled in the application.
        /// </summary>
        public bool EnablePhotos
        {
            get => CheckIsOnMainThread(_EnablePhotos);
            set => SetPropertyBackingValue(value, ref _EnablePhotos);
        }

        private bool _EnableGames = true;
        /// <summary>
        /// Whether or not Games are supported in the application.
        /// </summary>
        public bool EnableGames
        {
            get => CheckIsOnMainThread(_EnableGames);
            set => SetPropertyBackingValue(value, ref _EnableGames);
        }

        private bool _EnableWebAccess = true;
        /// <summary>
        /// Whether or not web access is supported by the application.
        /// </summary>
        public bool EnableWebAccess
        {
            get => CheckIsOnMainThread(_EnableWebAccess);
            set => SetPropertyBackingValue(value, ref _EnableWebAccess);
        }

        private bool _EnableApplications = true;
        /// <summary>
        /// Whether or not launching applications is enabled;
        /// </summary>
        public bool EnableApplications
        {
            get => CheckIsOnMainThread(_EnableApplications);
            set => SetPropertyBackingValue(value, ref _EnableApplications);
        }

        private bool _EnableWebSites = true;
        /// <summary>
        /// Whether or not launching web sites is enabled.
        /// </summary>
        public bool EnableWebSites
        {
            get => CheckIsOnMainThread(_EnableWebSites);
            set => SetPropertyBackingValue(value, ref _EnableWebSites);
        }

        private bool _EnableWidgets = true;
        /// <summary>
        /// Whether or not widgets (calendar, weather, etc) are enabled.
        /// </summary>
        public bool EnableWidgets
        {
            get => CheckIsOnMainThread(_EnableWidgets);
            set => SetPropertyBackingValue(value, ref _EnableWidgets);
        }
        #endregion
    }
}
