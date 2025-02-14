using Dissertation.Models.Challenge;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Project> Projects { get; set; }
    public DbSet<Scenario> Scenarios { get; set; }
    public DbSet<Choice> Choices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Scenario>()
            .Property(s => s.Phase)
            .HasConversion<int>();

        // Scenario -> Project relationship
        modelBuilder.Entity<Scenario>()
            .HasOne(s => s.Project)
            .WithMany(p => p.Scenarios)
            .HasForeignKey(s => s.ProjectId)
            .OnDelete(DeleteBehavior.Cascade); // If a project is deleted, delete scenarios

        // Choice -> Scenario relationship
        modelBuilder.Entity<Choice>()
            .HasOne(c => c.Scenario)
            .WithMany(s => s.Choices)
            .HasForeignKey(c => c.ScenarioId)
            .OnDelete(DeleteBehavior.Cascade); // If a scenario is deleted, delete choices

        // Choice -> NextScenario (Self-referencing)
        modelBuilder.Entity<Choice>()
            .HasOne(c => c.NextScenario)
            .WithMany()
            .HasForeignKey(c => c.NextScenarioId)
            .OnDelete(DeleteBehavior.SetNull); // If the next scenario is deleted, set to null
    }
}