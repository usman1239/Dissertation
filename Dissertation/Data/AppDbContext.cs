using Dissertation.Models.Challenge;
using Dissertation.Models.Challenge.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace Dissertation.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectInstance> ProjectInstances { get; set; }
    public DbSet<Sprint> Sprints { get; set; }
    public DbSet<UserStory> UserStories { get; set; }
    public DbSet<UserStoryInstance> UserStoryInstances { get; set; }
    public DbSet<Developer> Developers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Project → UserStory (One-to-Many)
        modelBuilder.Entity<UserStory>()
            .HasOne(us => us.Project)
            .WithMany(p => p.UserStories)
            .HasForeignKey(us => us.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // 2. Project → ProjectInstance (One-to-Many)
        modelBuilder.Entity<ProjectInstance>()
            .HasOne(pi => pi.Project)
            .WithMany()
            .HasForeignKey(pi => pi.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // 3. UserStory → UserStoryInstance (One-to-Many)
        modelBuilder.Entity<UserStoryInstance>()
            .HasOne(usi => usi.UserStory)
            .WithMany(us => us.UserStoryInstances)
            .HasForeignKey(usi => usi.UserStoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // 4. ProjectInstance → UserStoryInstance (One-to-Many)
        modelBuilder.Entity<UserStoryInstance>()
            .HasOne(usi => usi.ProjectInstance)
            .WithMany(pi => pi.UserStoryInstances)
            .HasForeignKey(usi => usi.ProjectInstanceId)
            .OnDelete(DeleteBehavior.Cascade);

        // 5. UserStoryInstance → Developer (Many-to-One)
        modelBuilder.Entity<UserStoryInstance>()
            .HasOne(usi => usi.DeveloperAssigned)
            .WithMany()
            .HasForeignKey(usi => usi.DeveloperAssignedId)
            .OnDelete(DeleteBehavior.SetNull);

        // 6. ProjectInstance → Sprint (One-to-Many)
        modelBuilder.Entity<Sprint>()
            .HasOne(s => s.ProjectInstance)
            .WithMany(pi => pi.Sprints)
            .HasForeignKey(s => s.ProjectInstanceId)
            .OnDelete(DeleteBehavior.Cascade);



        modelBuilder.Entity<Project>()
            .Property(p => p.DeveloperCosts)
            .HasConversion(
                v => JsonConvert.SerializeObject(v), // Serialize Dictionary to JSON string (default to empty string if null)
                v => (string.IsNullOrEmpty(v) ? new Dictionary<DeveloperExperienceLevel, int>() : JsonConvert.DeserializeObject<Dictionary<DeveloperExperienceLevel, int>>(v))! // Deserialize JSON string back to Dictionary (empty if null or empty string)
            )
            .Metadata.SetValueComparer(new ValueComparer<Dictionary<DeveloperExperienceLevel, int>>(
                (c1, c2) =>
                    (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.Count == c2.Count && !c1.Except(c2).Any()), // Ensure null checks and deep equality
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.Key.GetHashCode(), v.Value.GetHashCode())), // Proper hash code generation, ensuring null handling
                c => c.ToDictionary(k => k.Key, v => v.Value) // Copy the dictionary properly or return a new empty one if null
            ));


    }

    public async Task RemoveIdleDevelopersAsync()
    {
        // Find developers who are not assigned to any UserStoryInstance
        var idleDevelopers = await Developers
            .Where(dev => !UserStoryInstances
                .Any(usi => usi.DeveloperAssignedId == dev.Id))
            .ToListAsync();

        // Remove those developers from the database
        if (idleDevelopers.Any())
        {
            Developers.RemoveRange(idleDevelopers);
            await SaveChangesAsync();
        }
    }
}