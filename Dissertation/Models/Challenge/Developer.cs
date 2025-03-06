using Dissertation.Models.Challenge.Enums;

namespace Dissertation.Models.Challenge;

public class Developer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DeveloperExperienceLevel ExperienceLevel { get; set; }
    public int Cost { get; set; }
    public string UserId { get; set; } = null!;
    public List<ProjectInstance>? AssignedProjects { get; set; }
    public List<UserStory>? AssignedUserStories { get; set; } = [];
}