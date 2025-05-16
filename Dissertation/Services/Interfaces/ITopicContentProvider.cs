using Dissertation.Models.Learn;

namespace Dissertation.Services.Interfaces;

public interface ITopicContentProvider
{
    TopicContent GetContent(string topic);
}