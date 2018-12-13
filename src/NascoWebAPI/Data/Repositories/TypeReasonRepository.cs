using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class TypeReasonRepository : Repository<TypeReason>, ITypeReasonRepository
    {
        public TypeReasonRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}