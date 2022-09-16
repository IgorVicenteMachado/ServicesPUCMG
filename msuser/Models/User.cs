using Microsoft.AspNetCore.Identity;
using msuser.Enums;

namespace msuser.Models
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Profile Profile { get; set; }
        public IEnumerable<UserRole> Roles { get; set; } = new List<UserRole>();

    }
}
