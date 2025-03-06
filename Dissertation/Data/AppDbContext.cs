using Dissertation.Models.Challenge;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectInstance> UserProjectInstances { get; set; }
    public DbSet<Sprint> Sprints { get; set; }
    public DbSet<UserStory> UserStories { get; set; }
    public DbSet<UserStoryInstance> UserStoryInstances { get; set; }
    public DbSet<Developer> Developers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}