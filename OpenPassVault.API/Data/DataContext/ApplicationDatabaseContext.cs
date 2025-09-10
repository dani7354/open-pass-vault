using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenPassVault.API.Data.Entity;

namespace OpenPassVault.API.Data.DataContext;

public class ApplicationDatabaseContext(DbContextOptions<ApplicationDatabaseContext> options) : IdentityDbContext<ApiUser>(options)
{
    public virtual DbSet<ApiUser> ApiUser { get; set; } = null!;
    public virtual DbSet<Secret> Secret { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<ApiUser>().ToTable("ApiUser").HasKey(x => x.Id);
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("ApiUserRole").HasKey(x => new {x.RoleId, x.UserId});
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("ApiUserLogin").HasKey(x => x.UserId); 
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("ApiUserClaim").HasKey(x => x.Id); 
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("ApiUserToken").HasKey(x => x.UserId);
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim").HasKey(x => x.Id);
        modelBuilder.Entity<IdentityRole>().ToTable("Role").HasKey(x => x.Id); ;
    }
}