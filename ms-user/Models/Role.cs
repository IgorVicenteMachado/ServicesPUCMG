using Microsoft.AspNetCore.Identity;

namespace ms_user.Models
{
    public class Role : IdentityRole<Guid>
    {
        public IEnumerable<UserRole> Users { get; set; } = new List<UserRole>();
        //public ICollection<User> Users { get; set; } = new List<User>();

    }
}
