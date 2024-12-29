using Dissertation.Models;

namespace Dissertation.Components.Pages;

public class QuestionProvider : IQuestionProvider
{
    public List<Question> GetQuestions(string topic)
    {
        var questions = new List<Question>();

        switch (topic.ToUpper())
        {
            case "SCRUM":
                questions.Add(new Question
                {
                    QuestionTitle = "How many letters does Scrum have?",
                    Answer = "5",
                    PossibleOptions = new List<string> { "1", "2", "3", "5" }
                });

                questions.Add(new Question
                {
                    QuestionTitle = "What does Scrum mean?",
                    Answer = "44",
                    PossibleOptions = new List<string> { "12", "22", "44", "50" }
                });
                break;

            case "KANBAN":
                questions.Add(new Question
                {
                    QuestionTitle = "What is the origin of Kanban?",
                    Answer = "Japan",
                    PossibleOptions = new List<string> { "USA", "Germany", "Japan", "China" }
                });

                questions.Add(new Question
                {
                    QuestionTitle = "What is the primary purpose of Kanban?",
                    Answer = "Visualize workflow",
                    PossibleOptions = new List<string>
                        { "Increase profit", "Visualize workflow", "Add more team members", "Create deadlines" }
                });
                break;
        }

        return questions;
    }
}