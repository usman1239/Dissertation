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
            ["ReducedBudget"] = new()
            {
                Id = "ReducedBudget",
                Description = "Budget has been slashed by 5%.",
                Apply = state =>
                {
                    state.CurrentProjectInstance.Budget = (int)(state.CurrentProjectInstance.Budget * 0.95);
                }
            },

            ["CostSurge"] = new()
            {
                Id = "CostSurge",
                Description = "Developer costs are temporarily inflated by 5%.",
                Apply = _ => { }
            }
        };
    }

    public ChallengeModifier GetTodayChallenge()
    {
        var keys = _challenges.Keys.ToList();
        var index = DateTime.UtcNow.DayOfYear % keys.Count;
        var key = keys[index];

        return _challenges[key];
    }
}