using System.ComponentModel.DataAnnotations;

namespace Dissertation.Models.Challenge.Enums;

public enum UserStoryTaskDifficulty
{
    [Display(Name = "Easy")] Easy = 0,
    [Display(Name = "Medium")] Medium = 1,
    [Display(Name = "Hard")] Hard = 2
}