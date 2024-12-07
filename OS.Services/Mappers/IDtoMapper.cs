using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.Services.Mappers
{
    public interface IDtoMapper
    {
        T Map<T>(object source);
    }
}
