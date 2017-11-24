using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Models
{
    public class UserViewModel
    {

    }
    public class LoginModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [UIHint("password")]
        public string Password { get; set; }
        public string PasswordHash { get; set; }
    }


}
