using Dissertation.Models.Learn;

namespace Dissertation.Components.Pages.Learn;

public interface IQuestionProvider
{
    List<Question> GetQuestions(string topic);
}