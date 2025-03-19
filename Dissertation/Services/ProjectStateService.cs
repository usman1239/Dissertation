using System.Collections.ObjectModel;
using Dissertation.Models.Challenge;

namespace Dissertation.Services;

public class ProjectStateService
{
    public string? UserId { get; set; } = string.Empty;

    public ProjectInstance CurrentProjectInstance { get; set; } = null!;
    public ObservableCollection<Developer> Team { get; set; } = [];
    public ObservableCollection<UserStoryInstance> UserStoryInstances { get; set; } = [];
    public ObservableCollection<Sprint> Sprints { get; set; } = [];
}