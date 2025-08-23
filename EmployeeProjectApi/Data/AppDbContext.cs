using Microsoft.EntityFrameworkCore;
using EmployeeProjectApi.Models;

namespace EmployeeProjectApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<EmployeeProject> EmployeeProjects => Set<EmployeeProject>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // composite PK for the join table
        b.Entity<EmployeeProject>()
         .HasKey(ep => new { ep.EmployeeId, ep.ProjectId });
    }
}
