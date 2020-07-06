using NascoWebAPI.Data.Entities;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class UnitGroupRepository : Repository<UnitGroups>, IUnitGroupRepository
    {
        public UnitGroupRepository(ApplicationDbContext context) : base(context)
        {

        }
        public List<Proc_GetAllUnitGroup_Result> _GetAllUnitGroup()
        {
            return SqlHelper.ExecuteQuery<Proc_GetAllUnitGroup_Result>(this._context, "Proc_GetAllUnitGroup",null).ToList();
        }
    }
}
