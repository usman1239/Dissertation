using Dissertation.Models.Learn;
using Dissertation.Services.Interfaces;

namespace Dissertation.Services;

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
                    QuestionTitle = "What is the primary role of the Scrum Master?",
                    Answer = "Facilitate the Scrum process and remove obstacles",
                    PossibleOptions = new List<string>
                    {
                        "Manage the team and assign tasks",
                        "Facilitate the Scrum process and remove obstacles",
                        "Write code with the developers",
                        "Approve the final product"
                    }
                });

                questions.Add(new Question
                {
                    QuestionTitle = "What is the maximum duration of a Sprint in Scrum?",
                    Answer = "4 weeks",
                    PossibleOptions = new List<string>
                    {
                        "1 week",
                        "2 weeks",
                        "3 weeks",
                        "4 weeks"
                    }
                });

                questions.Add(new Question
                {
                    QuestionTitle = "Which artifact represents the work to be done in the Scrum project?",
                    Answer = "Product Backlog",
                    PossibleOptions = new List<string>
                    {
                        "Sprint Backlog",
                        "Product Backlog",
                        "Increment",
                        "Definition of Done"
                    }
                });

                questions.Add(new Question
                {
                    QuestionTitle = "Who is responsible for creating and prioritizing items in the Product Backlog?",
                    Answer = "Product Owner",
                    PossibleOptions = new List<string>
                    {
                        "Scrum Master",
                        "Development Team",
                        "Stakeholders",
                        "Product Owner"
                    }
                });

                questions.Add(new Question
                {
                    QuestionTitle = "What is the outcome of a Sprint?",
                    Answer = "A potentially shippable product increment",
                    PossibleOptions = new List<string>
                    {
                        "A complete set of documents",
                        "A potentially shippable product increment",
                        "A project plan",
                        "A list of completed tasks"
                    }
                });
                break;


            case "KANBAN":
                questions.Add(new Question
                {
                    QuestionTitle = "In Kanban, what is the purpose of limiting Work In Progress (WIP)?",
                    Answer = "To improve focus and flow",
                    PossibleOptions = new List<string>
                    {
                        "To make the board look less crowded",
                        "To increase the number of tasks assigned",
                        "To improve focus and flow",
                        "To speed up meetings"
                    }
                });

                questions.Add(new Question
                {
                    QuestionTitle = "In Kanban, how is progress typically visualized?",
                    Answer = "Using a board with columns for each stage of work",
                    PossibleOptions = new List<string>
                    {
                        "With weekly reports",
                        "Using a board with columns for each stage of work",
                        "Through daily team emails",
                        "With Gantt charts"
                    }
                });

                questions.Add(new Question
                {
                    QuestionTitle =
                        "When using Kanban, what should happen if too many tasks pile up in a single column?",
                    Answer = "Team should collaborate to address the bottleneck",
                    PossibleOptions = new List<string>
                    {
                        "Ignore it and continue",
                        "Add more columns",
                        "Team should collaborate to address the bottleneck",
                        "Move tasks backwards"
                    }
                });

                questions.Add(new Question
                {
                    QuestionTitle = "What is the main goal when managing tasks with Kanban?",
                    Answer = "Achieve a smooth and continuous workflow",
                    PossibleOptions = new List<string>
                    {
                        "Complete tasks as quickly as possible",
                        "Assign all tasks immediately",
                        "Achieve a smooth and continuous workflow",
                        "Prioritize urgent work only"
                    }
                });

                questions.Add(new Question
                {
                    QuestionTitle = "How should new work items be introduced in Kanban?",
                    Answer = "Only when capacity is available",
                    PossibleOptions = new List<string>
                    {
                        "Whenever new work is requested",
                        "Only at the start of the week",
                        "After a task is completed and capacity is available",
                        "During the daily stand-up"
                    }
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
                        "To deliver a product increment and a product"
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
                        "To deliver the project increment and the project"
                    }
                });
                break;
        }

        return questions;
    }
}