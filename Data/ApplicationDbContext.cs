using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarRentalIdentityServer.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            List<IdentityRole> roles = new List<IdentityRole> {
            new IdentityRole { Id = "b49e5e21-bcdb-4fac-b8ea-bfa2d81168f7", Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "0b5141f7-3aed-4cf9-a51d-4ad671703e1f", Name = "Customer", NormalizedName = "CUSTOMER" }
            };

            builder.Entity<IdentityRole>().HasData(roles);

            var pH = new PasswordHasher<IdentityUser>();
            var user = new IdentityUser
            {
                Id = "1b7fe7c6-fc40-4f0e-934e-7c83f9d75406",
                Email = "rar.cental@gmail.com",
                EmailConfirmed = true,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                NormalizedEmail = "RAR.CENTAL@GMAIL.COM",
                PhoneNumber = "773951604",
                PhoneNumberConfirmed = true,
            };
            user.PasswordHash = pH.HashPassword(user, "yo");
            builder.Entity<IdentityUser>().HasData(new List<IdentityUser> { user });

            var userRoles = new IdentityUserRole<string>
            {
                RoleId = "b49e5e21-bcdb-4fac-b8ea-bfa2d81168f7",
                UserId = "1b7fe7c6-fc40-4f0e-934e-7c83f9d75406"
            };
            builder.Entity<IdentityUserRole<string>>().HasData(userRoles);
        }
    }
}