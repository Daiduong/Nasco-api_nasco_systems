using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class RecipientRepository : Repository<Recipient> , IRecipientRepository
    {
        public RecipientRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
