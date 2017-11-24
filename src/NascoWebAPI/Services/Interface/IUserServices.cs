using NascoWebAPI.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using NascoWebAPI.Data;

namespace NascoWebAPI.Services.Interface
{
    public interface IUserServices
    {
        Task<dynamic> SignIn(IOfficerRepository officer, LoginModel lsvm);
    }
}
