using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace SheltonHTPC.Dtos.Layout
{
    /// <summary>
    /// Class for holding layout settings that apply to all scenes.
    /// </summary>
    public class LayoutSettingsDto
    {
        /// <summary>
        /// Name of the collection in its database file; use this when retrieving it from the repo.
        /// </summary>
        public static string CollectionName => nameof(LayoutSettingsDto);

        internal static void RegisterDtoMap(BsonMapper mapper)
        {
            mapper.Entity<LayoutSettingsDto>()
                .Id(x => x.Id)
                .Field(x => x.BackgroundRotationScheme, nameof(BackgroundRotationScheme));
        }

        public Guid Id { get; set; }

        /// <summary>
        /// The current background rotation scheme to be used in the front end application.
        /// </summary>
        public BackgroundRotationScheme BackgroundRotationScheme { get; set; }
    }
}
