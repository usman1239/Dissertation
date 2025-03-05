using Dissertation.Models.Challenge;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<UserProjectInstance> UserProjectInstances { get; set; }
    public DbSet<Sprint> Sprints { get; set; }
    public DbSet<UserStory> UserStories { get; set; }
    public DbSet<UserStoryTask> UserStoryTasks { get; set; }
    public DbSet<Developer> Developers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //UserProjectInstance->Project(One - to - Many)
        modelBuilder.Entity<UserProjectInstance>()
            .HasOne(up => up.Project)
            .WithMany() // No need for a navigation property on Project
            .HasForeignKey(up => up.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        // UserProjectInstance -> Sprint (One-to-Many)
        modelBuilder.Entity<Sprint>()
            .HasOne(s => s.ProjectInstance)
            .WithMany(p => p.Sprints)
            .HasForeignKey(s => s.ProjectInstanceId)
            .OnDelete(DeleteBehavior.Cascade);

        // UserProjectInstance -> UserStory (One-to-Many)
        modelBuilder.Entity<UserStory>()
            .HasOne(us => us.ProjectInstance)
            .WithMany(up => up.UserStories)
            .HasForeignKey(us => us.ProjectInstanceId)
            .OnDelete(DeleteBehavior.Cascade);


        // UserStory -> UserStoryTask (One-to-Many)
        modelBuilder.Entity<UserStoryTask>()
            .HasOne(ut => ut.UserStory)
            .WithMany(us => us.Tasks)
            .HasForeignKey(ut => ut.UserStoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // ✅ One-to-Many: Developer -> UserStories
        modelBuilder.Entity<UserStory>()
            .HasOne(us => us.AssignedTo)
            .WithMany(d => d.AssignedUserStories) // ✅ Developer can have multiple UserStories
            .HasForeignKey(us => us.AssignedToId)
            .OnDelete(DeleteBehavior.SetNull);

        // ✅ One-to-Many: Developer -> UserStoryTasks
        modelBuilder.Entity<UserStoryTask>()
            .HasOne(ust => ust.AssignedTo)
            .WithMany(d => d.AssignedUserStoryTasks) // ✅ Developer can have multiple UserStoryTasks
            .HasForeignKey(ust => ust.AssignedToId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}