using System.Reflection;
using Dissertation.Models.Challenge;
using Dissertation.Models.Challenge.Enums;
using Dissertation.Services;
using Xunit;

namespace Dissertation.Tests.Services;

public class DailyChallengeServiceTests
{
    private readonly DailyChallengeService _service = new();

    private readonly ProjectStateService _projectStateService = new()
    {
        Team = [],
        UserStoryInstances = [],
        Sprints = []
    };


    [Fact]
    public void GetTodayChallenge_ReturnsConsistentChallenge_ForSameDay()
    {
        // Arrange
        var expected = _service.GetTodayChallenge();

        // Act
        var actual = _service.GetTodayChallenge();

        // Assert
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Description, actual.Description);
    }

    [Fact]
    public void NoSeniorDevs_RemovesAllSeniorDevelopers()
    {
        // Arrange
        var modifier = GetModifierById("NoSeniorDevs");

        _projectStateService.Team =
        [
            new Developer { ExperienceLevel = DeveloperExperienceLevel.Senior },
            new Developer { ExperienceLevel = DeveloperExperienceLevel.MidLevel },
            new Developer { ExperienceLevel = DeveloperExperienceLevel.Senior }
        ];

        // Act
        modifier.Apply(_projectStateService);

        // Assert
        Assert.All(_projectStateService.Team, d => Assert.NotEqual(DeveloperExperienceLevel.Senior, d.ExperienceLevel));
    }

    [Fact]
    public void ReducedBudget_AppliesFivePercentReduction()
    {
        // Arrange
        var modifier = GetModifierById("ReducedBudget");

        _projectStateService.CurrentProjectInstance = new ProjectInstance { Budget = 1000 };
        // Act
        modifier.Apply(_projectStateService);

        // Assert
        Assert.Equal(950, _projectStateService.CurrentProjectInstance.Budget);
    }

    [Fact]
    public void CostSurge_Apply_DoesNotModifyState()
    {
        // Arrange
        var modifier = GetModifierById("CostSurge");
        _projectStateService.CurrentProjectInstance = new ProjectInstance { Budget = 1234 };
        _projectStateService.Team =
        [
            new Developer { ExperienceLevel = DeveloperExperienceLevel.Junior }
        ];

        // Act
        modifier.Apply(_projectStateService);

        // Assert – Nothing should change
        Assert.Equal(1234, _projectStateService.CurrentProjectInstance.Budget);
        Assert.Single(_projectStateService.Team);
    }

    private static ChallengeModifier GetModifierById(string id)
    {
        var allModifiers = new DailyChallengeService();
        return allModifiers
            .GetType()
            .GetField("_challenges", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(allModifiers) is Dictionary<string, ChallengeModifier> dict && dict.TryGetValue(id, out var mod)
            ? mod
            : throw new Exception($"Modifier '{id}' not found.");
    }
}