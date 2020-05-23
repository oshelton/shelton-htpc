using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheltonHTPC.Dtos.Layout
{
    /// <summary>
    /// Represents a single set of backround images for use with scenes.
    /// </summary>
    public sealed class BackgroundSetDto
    {
        /// <summary>
        /// Name of the collection in its database file; use this when retrieving it from the repo.
        /// </summary>
        public static string CollectionName => "BackgroundSets";

        internal static void RegisterDtoMap(BsonMapper mapper)
        {
            mapper.Entity<BackgroundSetDto>()
                .Id(x => x.Id)
                .Field(x => x.Name, nameof(Name))
                .Field(x => x.Style, nameof(Style))
                .Field(x => x.IsNext, nameof(IsNext))
                .Field(x => x.Images, nameof(Images));
        }

        /// <summary>
        /// Id of the background set object.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the background set.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Current style used by the BackgroundSet.
        /// </summary>
        public BackgroundSetStyle Style { get; set; }

        /// <summary>
        /// Whether this background set is the next one that will be used or not.
        /// </summary>
        public bool IsNext { get; set; }

        /// <summary>
        /// List of all background images in the set.
        /// </summary>
        public List<BackgroundSetImageDto> Images { get; set; }
    }
}
