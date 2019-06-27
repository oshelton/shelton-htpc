using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SheltonHTPC.Data;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SheltonHTPC.FrontEndData.Entites
{
	/// <summary>
	/// Immutable GeneralSettings class for the SheltonHTPC Front end.
	/// </summary>
	public class GeneralSettings : Interfaces.IGeneralSettings
	{
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

							results.DataPath = dataPathFileContents;
							results.IdleWaitMinutes = (uint)jsonObj[nameof(IdleWaitMinutes)];
						}
					}
					else
						throw new InvalidOperationException("General Settings not found.");

					return results;
				}
				else
				{
					throw new InvalidOperationException("General Settings not found.");
				}
			});
		}

		/// <summary>
		/// The path to the directory that contains the assets, settings, and databases for the application.
		/// </summary>
		public string DataPath { get; private set; }
		
		/// <summary>
		/// Number of minutes before the application becomes idle.
		/// </summary>
		public uint IdleWaitMinutes { get; private set; }
	}
}
