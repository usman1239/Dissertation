using Dissertation.Models.Learn;

namespace Dissertation.Services.Interfaces;

public interface IQuestionProvider
{
    List<Question> GetQuestions(string topic);
}