using LiteDB;
using System;

namespace LiteDBImageStorageExample
{
    public class MutableTestClass
    {
        public static void RegisterEntityMap(BsonMapper mapper)
        {
            mapper.Entity<MutableTestClass>()
                .Id(x => x.Id)
                .Field(x => x.Column, nameof(Column))
                .Field(x => x.Row, nameof(Row))
                .Field(x => x.Size, nameof(Size))
                .Field(x => x.Title, nameof(Title))
                .Field(x => x.ImageId, nameof(ImageId));
        }

        public MutableTestClass() { }

        public Guid Id { get; set; }

        public int Column { get; set; }
        public int Row { get; set; }
        public TileSize Size { get; set; }

        public string Title { get; set; }

        public string ImageId { get; set; }
    }
}
