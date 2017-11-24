using AutoMapper;
using Microsoft.Extensions.Logging;
namespace NascoWebAPI.Services
{
    public class BaseServices
    {
        public ILogger<dynamic> _logger;
        public IMapper _mapper;
        public BaseServices(ILogger<dynamic> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }
       
    }
}
