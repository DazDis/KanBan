using Microsoft.EntityFrameworkCore;

namespace ShapesUI.DataBase;

public class ApplicationDbContext : DbContext
{
    public virtual DbSet<UserEntity> Users { get; set; }  

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}
