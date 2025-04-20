using Dissertation.Models.Learn;

namespace Dissertation.Components.Pages.Learn;

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

            case "PLANNING":
                questions.Add(new Question
                {
                    QuestionTitle = "What is the purpose of planning?",
                    Answer = "To set goals",
                    PossibleOptions = new List<string>
                    {
                        "To set goals", "To waste time", "To make the team work harder", "To make the team work longer"
                    }
                });
                questions.Add(new Question
                {
                    QuestionTitle = "What is the purpose of a sprint?",
                    Answer = "To deliver a product increment",
                    PossibleOptions = new List<string>
                    {
                        "To deliver a product increment", "To deliver a product",
                        "To deliver a product increment and a product",
                        "To deliver a product increment and a product increment"
                    }
                });
                break;

            case "EXECUTION":
                questions.Add(new Question
                {
                    QuestionTitle = "What is the purpose of execution?",
                    Answer = "To deliver the project",
                    PossibleOptions = new List<string>
                    {
                        "To deliver the project", "To deliver the project increment",
                        "To deliver the project increment and the project",
                        "To deliver the project increment and the project increment"
                    }
                });
                break;
        }


        return questions;
    }
}