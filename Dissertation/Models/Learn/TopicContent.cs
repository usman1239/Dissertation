namespace Dissertation.Models.Learn;

public class TopicContent
{
    public string TopicName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string VideoPath { get; set; } = string.Empty;
    public List<string> Paragraphs { get; set; } = [];
    public List<string>? SummaryPoints { get; set; }
}