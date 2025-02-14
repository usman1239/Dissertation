namespace Dissertation.Models.Challenge;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Difficulty { get; set; }

    public List<Scenario> Scenarios { get; set; } = new();
}