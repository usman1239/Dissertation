namespace Dissertation.Models.Challenge;

public class ProjectInstance
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!; //which user does it link to
    public int ProjectId { get; set; } //which vanilla project does it link to
    public Project Project { get; set; } = null!;
    public int Budget { get; set; }
    public List<Sprint> Sprints { get; set; } = [];
    public List<UserStory> UserStories { get; set; } = [];
}