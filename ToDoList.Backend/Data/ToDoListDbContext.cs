using Microsoft.EntityFrameworkCore;
using ToDoList.Backend.Models;

namespace ToDoList.Backend.Data;

public class ToDoListDbContext : DbContext
{
    public ToDoListDbContext(DbContextOptions<ToDoListDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<List> Lists { get; set; }
    public DbSet<Item> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(200);
            entity.Property(e => e.IsAdmin).IsRequired().HasDefaultValue(false);
            
            // Create unique index on Username
            entity.HasIndex(e => e.Username).IsUnique();
        });

        // Configure List entity
        modelBuilder.Entity<List>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.Owner)
                  .WithMany(u => u.Lists)
                  .HasForeignKey(e => e.OwnerID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Item entity
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(300);
            entity.HasOne(e => e.List)
                  .WithMany(l => l.Items)
                  .HasForeignKey(e => e.ListId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}