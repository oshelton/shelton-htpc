using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheltonHTPC.Dtos.Layout
{
    public sealed class BackgroundSetImageDto
    {
        internal static void RegisterDtoMap(BsonMapper mapper)
        {
            mapper.Entity<BackgroundSetImageDto>()
                .Field(x => x.SourcePath, nameof(SourcePath))
                .Field(x => x.UHDImagePath, nameof(UHDImagePath))
                .Field(x => x.HDImagePath, nameof(HDImagePath))
                .Field(x => x.FullImagePath, nameof(FullImagePath));
        }

        /// <summary>
        /// Path to the original image file.  May be helpful.
        /// </summary>
        public string SourcePath { get; set; }

        /// <summary>
        /// Path to the 4k appropriate image to use in FileStorage.  May be null if the source image was not large enough.
        /// </summary>
        public string UHDImagePath { get; set; }

        /// <summary>
        /// Path to the 1080p appropriate image to use in FileStorage.  Should not be null.
        /// </summary>
        public string HDImagePath { get; set; }

        /// <summary>
        /// Path to the thumbnail image to use in FileStorage.  Should not be null.  Should have a height of 128 pixels.
        /// </summary>
        public string FullImagePath { get; set; }
    }
}
