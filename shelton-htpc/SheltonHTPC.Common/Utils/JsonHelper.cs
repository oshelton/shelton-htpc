using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheltonHTPC.Common.Utils
{
    /// <summary>
    /// Helper class for common json operations.
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Get the value at the passed in key or the default value for the type or a specified default value.
        /// </summary>
        public static T GetValueOrDefault<T>(this JObject jsonObj, string key, T defaultValue = default(T))
        {
            JToken val = jsonObj[key];

            if (val != null)
                return val.Value<T>();
            else
                return defaultValue;
        }
    }
}
