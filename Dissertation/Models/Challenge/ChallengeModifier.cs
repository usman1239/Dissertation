namespace Dissertation.Models.Challenge;

public class ChallengeModifier
{
    public string Id { get; set; } = "";
    public string Description { get; set; } = "";
    public Action<ProjectStateService> Apply { get; set; } = _ => { };
}