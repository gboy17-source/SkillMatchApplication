using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.JsonWebTokens;

namespace SkillMatchApplication.Services
{
    internal class JwtService
    {
        public class JwtPayload
        {
            public string Id { get; set; }
            public string Role { get; set; }
            public long Exp { get; set; }
        }

        public static JwtPayload Decode(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;

            var handler = new JsonWebTokenHandler();
            var jwt = handler.ReadJsonWebToken(token);

            var payload = new JwtPayload
            {
                Id = jwt.GetClaim("id")?.Value,
                Role = jwt.GetClaim("role")?.Value,
                Exp = long.Parse(jwt.GetClaim("exp")?.Value)
            };

            return payload;
        }

        public static bool IsTokenExpired(string token)
        {
            var payload = Decode(token);
            if (payload == null) return true;
            var expDate = DateTimeOffset.FromUnixTimeSeconds(payload.Exp).UtcDateTime;
            return expDate < DateTime.UtcNow;
        }
    }
}