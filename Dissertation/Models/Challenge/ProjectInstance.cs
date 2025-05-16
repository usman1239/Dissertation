namespace Dissertation.Models.Challenge;

public class ProjectInstance
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public int Budget { get; set; }
    public ICollection<Sprint> Sprints { get; set; } = [];
    public ICollection<UserStoryInstance> UserStoryInstances { get; set; } = [];
}