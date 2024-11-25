using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Data.Models
{
    public enum StoredEntityType
    {
        Stream,
        Origin,
        AlbumCover,
        AlbumHelper,
        ArtistPicture,
        ArtistHelper,
    }
    public class StoredEntity : BaseModel
    {
        public StoredEntityType Type { get; set; }
        public required string Mime { get; set; }
        public Artist? Artist { get; set; }
        public Album? Album { get; set; }
        public Track? Track { get; set; }
    }
}
