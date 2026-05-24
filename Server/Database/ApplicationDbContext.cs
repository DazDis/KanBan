using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Server.Database.Entities;
using System.Runtime.CompilerServices;

namespace Server.DataBase;

public class ApplicationDbContext : DbContext
{
    public virtual DbSet<UserEntity> Users { get; set; }  
    public virtual DbSet<LabelEntity> Labels { get; set; }  
    public virtual DbSet<TaskEntity> Tasks { get; set; }
    public virtual DbSet<ColumnEntity> Columns { get; set; }
    public virtual DbSet<TeamEntity> Teams { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        // раскомментировать, чтобы уронить БД
        //Database.EnsureDeleted(); 
        Database.EnsureCreated();
    }

    // конвертирует время из БД(UTC) в локальное
    [ModuleInitializer]
    public static void Initialize()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
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
        // task и users
        modelBuilder.Entity<TaskEntity>()
            .HasMany(u => u.Teams)
            .WithMany(t => t.Tasks)
            .UsingEntity(j => j.ToTable("TaskTeams"));

        modelBuilder.Entity<UserEntity>()
            .HasMany(l => l.Teams)
            .WithMany(t => t.Users)
            .UsingEntity(j => j.ToTable("UserTeams"));

        modelBuilder.Entity<TaskEntity>()
            .HasOne(t => t.Column)
            .WithMany(c => c.Tasks)
            .HasForeignKey(t => t.ColumnId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}