namespace Dissertation.Models.Challenge;

public class UserStory
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int StoryPoints { get; set; }
    public ICollection<UserStoryInstance> UserStoryInstances { get; set; } = [];
}