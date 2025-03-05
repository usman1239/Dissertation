namespace Dissertation.Models.Challenge;

public class UserStory
{
    public int Id { get; set; }
    public int ProjectInstanceId { get; set; }
    public UserProjectInstance ProjectInstance { get; set; } = null!;
    public string Title { get; set; } = "";
    public int StoryPoints { get; set; }
    public int? AssignedToId { get; set; }
    public Developer? AssignedTo { get; set; }
    public List<UserStoryTask> Tasks { get; set; } = [];
    public bool IsCompleted { get; set; }
}