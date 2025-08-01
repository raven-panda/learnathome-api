using LearnAtHomeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnAtHomeApi;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<StudentTaskModel> StudentTasks => Set<StudentTaskModel>();
}