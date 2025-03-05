using Dissertation.Models.Challenge.Enums;

namespace Dissertation.Models.Challenge;

public class UserStoryTask
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public UserStoryTaskType Type { get; set; }
    public int? AssignedToId { get; set; }
    public Developer? AssignedTo { get; set; }
    public int UserStoryId { get; set; }
    public UserStory UserStory { get; set; } = null!;
    public UserStoryTaskDifficulty Difficulty { get; set; }
    public bool IsCompleted { get; set; }
    public double Progress { get; set; }
}