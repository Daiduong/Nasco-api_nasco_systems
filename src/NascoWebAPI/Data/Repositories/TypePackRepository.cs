using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class TypePackRepository : Repository<TypePack> , ITypePackRepository
    {
        public TypePackRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
