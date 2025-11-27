using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillMatchApplication.Services
{
    internal class Session
    {
        // Raw JWT string for API requests
        public static string JwtToken { get; set; }

        // Decoded JWT payload for easy access to user info
        public static JwtService.JwtPayload User { get; set; }
    }
}