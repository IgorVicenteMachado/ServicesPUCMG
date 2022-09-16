using Microsoft.AspNetCore.Identity;

namespace msuser.Models
{
    public class Role : IdentityRole<Guid>
    {
        public IEnumerable<UserRole> Users { get; set; } = new List<UserRole>();

    }
}
