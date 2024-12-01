using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Data.Schemas
{
    public class CreatePlaylistBody
    {
        public required string Name { get; set; }

        public IEnumerable<Guid> Tracks { get; set; } = [];
    }
}
