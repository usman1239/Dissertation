using Dissertation.Models.Challenge.Enums;

namespace Dissertation.Models.Challenge;

public class Project
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Budget { get; set; }
    public int NumOfSprints { get; set; }
    public ICollection<UserStory> UserStories { get; set; } = [];
    public Dictionary<DeveloperExperienceLevel, int> DeveloperCosts { get; set; } = [];
}