using LearnAtHomeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnAtHomeApi;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<StudentTaskModel> StudentTasks => Set<StudentTaskModel>();
    public DbSet<RpUserModel> Users => Set<RpUserModel>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RpUserModel>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}