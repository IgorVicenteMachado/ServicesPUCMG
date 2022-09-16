using ms_user.Extensions;
using ms_user.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ms_user.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ms_user.Services
{
    public class TokenService : ITokenService
    {
    
        private readonly UserManager<User> _userManager;
        public readonly SymmetricSecurityKey _key;

        public TokenService( IConfiguration config, UserManager<User> userManager )
        {
            _userManager = userManager;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public async Task<string> CreateToken(User userx)
        {
            //var user = _mapper.Map<User>(userx);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userx.Id.ToString()),
                new Claim(ClaimTypes.Name, userx.UserName)
            };

            var roles = await _userManager.GetRolesAsync(userx);

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescription);

            return tokenHandler.WriteToken(token);
        }
    }
}


//public string GenerateToken(User user)
//{
//    var tokenHandler = new JwtSecurityTokenHandler();

//    var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);

//    var claims = user.GetClaims();

//    var tokenDescriptor = new SecurityTokenDescriptor
//    {
//        Subject = new ClaimsIdentity(claims),
//        Expires = DateTime.UtcNow.AddHours(1),
//        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//    };

//    var token = tokenHandler.CreateToken(tokenDescriptor);
//    return tokenHandler.WriteToken(token);
//}