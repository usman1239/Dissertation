namespace Dissertation.Models;

public class Question
{
    public string QuestionTitle { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public IEnumerable<string> PossibleOptions { get; set; } = [];
}