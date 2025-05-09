using Dissertation.Models.Challenge;
using Dissertation.Models.Challenge.Enums;
using Dissertation.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using MudBlazor;

namespace Dissertation.View_Models;

public class DeveloperManagementViewModel(
    ProjectStateService projectStateService,
    IDeveloperService developerService,
    ISnackbar snackbar)
{
    public string DeveloperName { get; set; } = "";
    public DeveloperExperienceLevel SelectedDeveloperExperienceLevel { get; set; } = DeveloperExperienceLevel.Junior;

    public bool CanAddDeveloper()
    {
        if (projectStateService.ActiveChallenge?.Id != "NoSeniorDevs" ||
            SelectedDeveloperExperienceLevel != DeveloperExperienceLevel.Senior)
            return !DeveloperName.IsNullOrEmpty();

        snackbar.Add("You can't hire Senior developers today due to the active challenge!", Severity.Error);
        return false;
    }

    public async Task AddDeveloper()
    {
        if (string.IsNullOrWhiteSpace(DeveloperName))
        {
            snackbar.Add("Developer name cannot be empty!", Severity.Warning);
            return;
        }

        if (projectStateService.Team.Any(dev => dev.Name.Equals(DeveloperName, StringComparison.OrdinalIgnoreCase)))
        {
            snackbar.Add($"A developer named {DeveloperName} already exists in your team!", Severity.Warning);
            return;
        }

        var baseCost = GetDeveloperCost();

        var dev = new Developer
        {
            Name = DeveloperName,
            ExperienceLevel = SelectedDeveloperExperienceLevel,
            Cost = baseCost,
            UserId = projectStateService.UserId!
        };

        await developerService.AddDeveloperAsync(dev);

        projectStateService.Team.Add(dev);
        snackbar.Add($"Developer {DeveloperName} added successfully!", Severity.Success);
        DeveloperName = "";
    }

    private int GetDeveloperCost()
    {
        return SelectedDeveloperExperienceLevel switch
        {
            DeveloperExperienceLevel.Junior => projectStateService.CurrentProjectInstance.Project.DeveloperCosts
                .TryGetValue(DeveloperExperienceLevel.Junior, out var juniorCost)
                ? juniorCost
                : 0,

            DeveloperExperienceLevel.MidLevel => projectStateService.CurrentProjectInstance.Project.DeveloperCosts
                .TryGetValue(DeveloperExperienceLevel.MidLevel, out var midCost)
                ? midCost
                : 0,

            DeveloperExperienceLevel.Senior => projectStateService.CurrentProjectInstance.Project.DeveloperCosts
                .TryGetValue(DeveloperExperienceLevel.Senior, out var seniorCost)
                ? seniorCost
                : 0,

            _ => throw new ArgumentOutOfRangeException(nameof(SelectedDeveloperExperienceLevel))
        };
    }


    public async Task RemoveDeveloper(Developer dev)
    {
        var userStoryInstancesAssignedToDeveloper = projectStateService.UserStoryInstances
            .Where(usi => usi.DeveloperAssignedId == dev.Id).ToList();

        foreach (var usi in userStoryInstancesAssignedToDeveloper)
        {
            usi.DeveloperAssignedId = null;
            usi.DeveloperAssigned = null;
        }

        await developerService.RemoveDeveloperAsync(dev);
        projectStateService.Team.Remove(dev);
    }
}