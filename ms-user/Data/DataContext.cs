using ms_user.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ms_user.Data
{
    public class DataContext : IdentityDbContext<User, Role, Guid, 
        IdentityUserClaim<Guid>,
        UserRole, 
        IdentityUserLogin<Guid>, 
        IdentityRoleClaim<Guid>, 
        IdentityUserToken<Guid>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration(new UserMap());
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

                userRole.HasOne(ur => ur.User)
                .WithMany(r => r.Roles)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
            });
        }
    }
}


//modelBuilder.Entity<User>()
//    .HasMany(x => x.Roles)
//    .WithMany(x => x.Users)
//    .UsingEntity<Dictionary<string, object>>(
//        "UserRole",
//        role => role
//            .HasOne<Role>()
//            .WithMany()
//            .HasForeignKey("RoleId")
//            .HasConstraintName("FK_UserRole_RoleId")
//            .OnDelete(DeleteBehavior.Cascade),
//        user => user
//            .HasOne<User>()
//            .WithMany()
//            .HasForeignKey("UserId")
//            .HasConstraintName("FK_UserRole_UserId")
//            .OnDelete(DeleteBehavior.Cascade));
