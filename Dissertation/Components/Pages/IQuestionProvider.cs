using Dissertation.Models;

namespace Dissertation.Components.Pages;

public interface IQuestionProvider
{
    List<Question> GetQuestions(string topic);
}