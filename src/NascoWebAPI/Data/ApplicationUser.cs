using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string OfficerName { get; set; }
    }
}
