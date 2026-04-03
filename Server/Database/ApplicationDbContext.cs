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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // использование Fluent API
        base.OnModelCreating(modelBuilder);

        // task и labels 
        modelBuilder.Entity<TaskEntity>()
            .HasMany(l => l.Labels)
            .WithMany(t => t.Tasks)
            .UsingEntity(j => j.ToTable("TaskLabels"));

        // task и users
        modelBuilder.Entity<TaskEntity>()
            .HasMany(u => u.Users)
            .WithMany(t => t.Tasks)
            .UsingEntity(j => j.ToTable("TaskUsers"));
    }
}
