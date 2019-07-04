using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace SheltonHTPC.Dtos.Layout
{
    /// <summary>
    /// Class for providing access to Layout and scene data.
    /// </summary>
    public static class LayoutDataManager
    {
        static LayoutDataManager()
        {
            _TypeMapper = new BsonMapper();
            LayoutSettingsDto.RegisterDtoMap(_TypeMapper);
        }

        /// <summary>
        /// Update the application's data directory.
        /// </summary>
        /// <remarks>This must be set before Repo is accessed.</remarks>
        public static void UpdateDataDirectory(string newDirectory)
        {
            _DatabaseFullPath = Path.Combine(newDirectory, _DatabaseFileName);
        }

        /// <summary>
        /// Get the repo containing layout and scene data.
        /// </summary>
        /// <remarks>Do not hold references to the object returned by this property; otherwise cache experition may cause it to be disposed while still in use.</remarks>
        public static LiteRepository Repo
        {
            get
            {
                if (_DatabaseFullPath == null)
                    throw new InvalidOperationException("Path to data directory has not been set.  Set via LayoutDataManager.UpdateDataDirectory.");

                var cache = MemoryCache.Default;
                if (cache.Contains(_CachedRepoName))
                    return cache.Get(_CachedRepoName) as LiteRepository;
                else
                {
                    var repo = new LiteRepository($"{_DatabaseFullPath}", _TypeMapper);
                    var evictionPolicy = new CacheItemPolicy()
                    {
                        RemovedCallback = RemovedCallback,
                        SlidingExpiration = TimeSpan.FromMinutes(2)
                    };
                    MemoryCache.Default.Set(_CachedRepoName, repo, evictionPolicy);
                    return repo;
                }
            }
        }

        /// <summary>
        /// Handles disposing of the repo when evicted from the cache.
        /// </summary>
        private static void RemovedCallback(CacheEntryRemovedArguments arg)
        {
            if (arg.RemovedReason != CacheEntryRemovedReason.Removed)
            {
                if (arg.CacheItem.Value is IDisposable item)
                    item.Dispose();
            }
        }

        /// <summary>
        /// Mapper containing the various types included in the Layout database.
        /// </summary>
        private static BsonMapper _TypeMapper;

        /// <summary>
        /// Name of the database file.
        /// </summary>
        private static string _DatabaseFileName = "Layout.db";

        /// <summary>
        /// Full path to the database file.
        /// </summary>
        private static string _DatabaseFullPath = null;

        private static readonly string _CachedRepoName = $"{nameof(LayoutDataManager)}.{nameof(LayoutDataManager.Repo)}"; 
    }
}
