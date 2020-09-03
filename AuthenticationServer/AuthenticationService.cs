using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationServer
{
    public interface IAuthenticateService
    {
        User Authenticate(string user, string password);
    }
    public class AuthenticationService:IAuthenticateService
    {
        private List<User> _users = new List<User>
        {
            new User{ Id=1, Name="admin",Password="admin",Role= Roles.Admin},
            new User{ Id=2, Name="customer",Password="customer",Role= Roles.Customer},
            new User{ Id=3, Name="deliver",Password="deliver",Role= Roles.Deliver},
        };

        private readonly AppSettings _appSettings;

        public AuthenticationService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public User Authenticate(string name,string password)
        {
            var user = _users.SingleOrDefault(x => x.Name == name && x.Password == password);
            if (user == null)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                   new Claim(ClaimTypes.Name,user.Name.ToString()),
                   new Claim("UserID",user.Id.ToString()),
                   new Claim(ClaimTypes.Role, user.Role)

               }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            user.Password = null;
            return user;

        }
    }
}
