using Microsoft.EntityFrameworkCore;

namespace Server.DataBase;

public class ApplicationDbContext : DbContext
{
    public virtual DbSet<UserEntity> Users { get; set; }  
    public virtual DbSet<LabelEntity> Labels { get; set; }  
    public virtual DbSet<TaskEntity> Tasks { get; set; }
    public virtual DbSet<ColumnEntity> Columns { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        // раскомментировать, чтобы уронить БД
        //Database.EnsureDeleted(); 
        Database.EnsureCreated();
    }
}
