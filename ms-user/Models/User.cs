using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace ms_user.Models
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Profile Perfil { get; set; }
        public IEnumerable<UserRole> Roles { get; set; } = new List<UserRole>();
       // public ICollection<Role> Roles { get; set; } = new List<Role>();

    }
}
