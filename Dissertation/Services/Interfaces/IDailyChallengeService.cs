using Dissertation.Models.Challenge;

namespace Dissertation.Services.Interfaces;

public interface IDailyChallengeService
{
    ChallengeModifier GetTodayChallenge();
}