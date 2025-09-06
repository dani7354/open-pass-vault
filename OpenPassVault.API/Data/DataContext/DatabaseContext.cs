using Microsoft.EntityFrameworkCore;
using OpenPassVault.API.Data.Entity;

namespace OpenPassVault.API.Data.DataContext;

public class DatabaseContext : DbContext
{
    public virtual DbSet<ApiUser> ApiUser { get; set; } = null!;
    
    public DatabaseContext() { }
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
}