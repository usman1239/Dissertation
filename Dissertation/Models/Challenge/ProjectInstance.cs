namespace Dissertation.Models.Challenge;

public class ProjectInstance
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!; //which user does it link to

    public int ProjectId { get; set; } //which vanilla project does it link to
    public Project Project { get; set; } = null!;

    public int Budget { get; set; }
    public ICollection<Sprint> Sprints { get; set; } = [];
    public ICollection<UserStoryInstance> UserStoryInstances { get; set; } = [];
    public string? LastAppliedChallengeKey { get; set; }
    public DateOnly? LastChallengeDate { get; set; }

}