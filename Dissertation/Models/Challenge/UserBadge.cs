using Dissertation.Models.Challenge.Enums;
using Microsoft.AspNetCore.Identity;

namespace Dissertation.Models.Challenge;

public class UserBadge
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public virtual IdentityUser User { get; set; } = null!;
    public BadgeType BadgeType { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}