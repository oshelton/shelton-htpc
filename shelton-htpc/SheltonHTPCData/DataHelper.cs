using System;
using System.IO;

namespace SheltonHTPC.Data
{
    /// <summary>
    /// Contains some helper properties and methods for accessing Shelton HTPC data.
    /// </summary>
    public class DataHelper
    {
        /// <summary>
        /// Path to the file containing the path to where the application data is actually stored.
        /// </summary>
        public static string DataPathFilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SheltonHTPC", "DataPath.txt");
    }
}
