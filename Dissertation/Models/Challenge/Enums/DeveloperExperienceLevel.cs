using System.ComponentModel.DataAnnotations;

namespace Dissertation.Models.Challenge.Enums;

public enum DeveloperExperienceLevel
{
    [Display(Name = "Junior")]
    Junior = 0,

    [Display(Name = "Mid-Level")]
    MidLevel = 1,

    [Display(Name = "Senior")]
    Senior = 2
}