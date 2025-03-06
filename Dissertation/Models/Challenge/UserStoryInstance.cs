namespace Dissertation.Models.Challenge;

public class UserStoryInstance
{
    public int Id { get; set; }
    public int UserStoryId { get; set; }
    public int Progress { get; set; }
    public int DeveloperAssignedId { get; set; }
    public Developer? DeveloperAssigned { get; set; }
    public bool IsComplete { get; set; }
}