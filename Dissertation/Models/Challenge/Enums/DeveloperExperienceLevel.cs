using System.ComponentModel.DataAnnotations;

namespace Dissertation.Models.Challenge.Enums;

public enum DeveloperExperienceLevel
{
    [Display(Name = "Junior (£1000 per sprint)")]
    Junior = 0,

    [Display(Name = "Mid-Level (£4000 per sprint)")]
    MidLevel = 1,

    [Display(Name = "Senior (£3000 per sprint)")]
    Senior = 2
}