using Dissertation.Models.Challenge;
using Dissertation.Models.Challenge.Enums;
using Dissertation.Services.Interfaces;

namespace Dissertation.Services;

public class DailyChallengeService : IDailyChallengeService
{
    private readonly Dictionary<string, ChallengeModifier> _challenges;

    public DailyChallengeService()
    {
        _challenges = new Dictionary<string, ChallengeModifier>
        {
            ["NoSeniorDevs"] = new()
            {
                Id = "NoSeniorDevs",
                Description = "No senior developers are available today.",
                Apply = state =>
                {
                    var seniors = state.Team.Where(d => d.ExperienceLevel == DeveloperExperienceLevel.Senior).ToList();
                    foreach (var senior in seniors) state.Team.Remove(senior);
                }
            },
            ["HalfBudget"] = new()
            {
                Id = "HalfBudget",
                Description = "Budget has been slashed by 50%.",
                Apply = state =>
                {
                    state.CurrentProjectInstance.Budget = (int)(state.CurrentProjectInstance.Budget * 0.5);
                }
            },
            ["LegacyCode"] = new()
            {
                Id = "LegacyCode",
                Description = "All user stories are at least 6 story points.",
                Apply = state =>
                {
                    foreach (var usi in state.UserStoryInstances)
                        if (usi.UserStory.StoryPoints < 6)
                            usi.UserStory.StoryPoints = 6;
                }
            }
            // Add more as desired
        };
    }

    public ChallengeModifier GetTodayChallenge()
    {
        // Deterministic rotation based on day of year
        var keys = _challenges.Keys.ToList();
        var index = DateTime.UtcNow.DayOfYear % keys.Count;
        var key = keys[index];

        return _challenges[key];
    }
}