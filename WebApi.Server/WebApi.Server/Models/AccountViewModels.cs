using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Server.Models
{
    public class UserLoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserRegisterViewModel
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ImageBase64 { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// Інформація про одного користувача
    /// </summary>
    public class UserItemViewModel
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public bool EmailConfirmed { get; set; }
        public int Age { get; set; }
    }
}
