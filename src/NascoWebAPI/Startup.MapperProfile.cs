using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NascoWebAPI.Helper.AutoMapperHelper;

namespace NascoWebAPI
{
    public partial class Startup
    {
        public IMapper Mapper { get; set; }
        private MapperConfiguration _mapperConfig { get; set; }
        private void AutoMapperCfg(IServiceCollection services)
        {
            _mapperConfig = new MapperConfiguration(
                cfg => cfg.AddProfile(new AutoMapperProfileConfiguration()));
            services.AddSingleton<IMapper>(sp => _mapperConfig.CreateMapper()); 
        }

    }
}
