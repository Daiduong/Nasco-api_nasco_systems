using Microsoft.Extensions.Logging;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class TypeOfPackRepository : Repository<TypeOfPack>, ITypeOfPackRepository
    {
        public TypeOfPackRepository(ApplicationDbContext context) : base(context)
        {

        }
        public TypeOfPack GetPriceDGHH(int? typePackID = 0, double? _long = 0, double? _width = 0, double? _height = 0)
        {
            if ((typePackID?? 0) > 0)
            {
                double average = ((_long ?? 0) + (_width??0) + (_height ?? 0)) / 3;
                Expression<Func<TypeOfPack, bool>> predicate = r => r.TypePackID == typePackID && (((r.Width_Product ?? 0) + (r.Height_Product ?? 0) + (r.Length_Product ?? 0)) / 3) >= average;
                if (this.Any(predicate))
                {
                    return this.GetFirst(predicate, o => o.OrderBy(or => or.Packing_Cartons));
                }
            }
            return new TypeOfPack();
        }
    }
}
