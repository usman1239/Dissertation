namespace Dissertation.Models.Challenge;

public class UserProjectInstance
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!; //which user does it link to
    public int ProjectId { get; set; } //which vanilla project does it link to
    public int Budget { get; set; }
    public Project Project { get; set; } = null!;
    public List<Sprint> Sprints { get; set; } = [];
    public List<UserStory> UserStories { get; set; } = [];
}