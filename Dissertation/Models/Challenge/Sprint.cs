namespace Dissertation.Models.Challenge;

public class Sprint
{
    public int Id { get; set; }
    public int ProjectInstanceId { get; set; }
    public ProjectInstance ProjectInstance { get; set; } = null!;
    public int SprintNumber { get; set; }
    public int Duration { get; set; }
    public bool IsCompleted { get; set; }
    public string Summary { get; set; } = string.Empty;
}