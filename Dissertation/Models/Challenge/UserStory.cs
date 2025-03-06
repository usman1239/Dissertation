namespace Dissertation.Models.Challenge;

public class UserStory
{
    public int Id { get; set; }
    public int ProjectInstanceId { get; set; }
    public ProjectInstance ProjectInstance { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int StoryPoints { get; set; }
}