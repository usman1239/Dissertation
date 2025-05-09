namespace Dissertation.Models.Challenge
{
    public class DailyChallengeCompletion
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int ProjectInstanceId { get; set; }
        public DateOnly Date { get; set; }
        public string ChallengeKey { get; set; } = null!;
    }
}
