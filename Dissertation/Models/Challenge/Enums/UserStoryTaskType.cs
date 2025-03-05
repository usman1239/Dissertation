using System.ComponentModel.DataAnnotations;

namespace Dissertation.Models.Challenge.Enums;

public enum UserStoryTaskType
{
    [Display(Name = "Bug")] Bug = 0,
    [Display(Name = "Backlog Item")] BacklogItem = 1
}