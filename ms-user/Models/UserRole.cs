﻿using Microsoft.AspNetCore.Identity;

namespace ms_user.Models
{
    public class UserRole : IdentityUserRole<Guid>
    {
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
