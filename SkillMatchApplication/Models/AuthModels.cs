using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillMatchApplication.Models
{
    internal class AuthModels
    {
        public class LoginRequest
        {
            public string email { get; set; }
            public string password { get; set; }
        }

        public class RegisterRequest
        {
            public string email { get; set; }
            public string password { get; set; }
        }

        public class LoginResponse
        {
            public string message { get; set; }
            public string token { get; set; }
        }

        public class RegisterResponse
        {
            public string message { get; set; }
            public string user { get; set; }
        }
    }
}