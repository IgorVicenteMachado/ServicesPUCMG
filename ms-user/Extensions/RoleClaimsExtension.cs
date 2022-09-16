using ms_user.Models;
using System.Security.Claims;

namespace ms_user.Extensions
{
    public static class RoleClaimsExtension
    {
        public static IEnumerable<Claim> GetClaims(this User user)
        {
            var result = new List<Claim>
            {
                new ( ClaimTypes.Name, user.FirstName),
                new( ClaimTypes.Email, user.Email),
                new Claim("user_id", user.Id.ToString())
            };

            result.AddRange(
               user.Roles.Select(role => new Claim(ClaimTypes.Role, role.RoleId.ToString()))) ;

            return result;
        }

    }
}
