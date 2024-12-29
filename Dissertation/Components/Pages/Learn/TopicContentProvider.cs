using Dissertation.Models;
using Dissertation.Models.Interfaces;

namespace Dissertation.Components.Pages.Learn;
public class TopicContentProvider : ITopicContentProvider
{
    private readonly Dictionary<string, TopicContent> _topics = new()
    {
        {
            "SCRUM",
            new TopicContent
            {
                TopicName = "Scrum",
                Description = "Scrum is an Agile methodology focused on iterative and incremental development.",
                VideoPath = "Videos/scrum.mp4",
                Paragraphs =
                [
                    "Scrum is a framework that allows teams to work together efficiently.",
                    "It encourages collaboration, accountability, and iterative progress."
                ],
                Questions =
                [
                    new Question
                    {
                        QuestionTitle = "How many letters does Scrum have?",
                        Answer = "5",
                        PossibleOptions = new List<string> { "1", "2", "3", "5" }
                    },

                    new Question
                    {
                        QuestionTitle = "What is Scrum primarily used for?",
                        Answer = "Agile Development",
                        PossibleOptions = new List<string> { "Waterfall", "Agile Development", "Testing", "Deployment" }
                    }
                ]
            }
        },
        {
            "KANBAN",
            new TopicContent
            {
                TopicName = "Kanban",
                Description = "Kanban is a visual system for managing work as it moves through a process.",
                VideoPath = "Videos/kanban.mp4",
                Paragraphs =
                [
                    "Kanban helps you visualise your work, maximise efficiency, and improve continuously.",
                    "It is widely used in software development and other industries."
                ],
                Questions =
                [
                    new Question
                    {
                        QuestionTitle = "What does Kanban emphasize?",
                        Answer = "Visualising Work",
                        PossibleOptions = new List<string> { "Coding", "Visualising Work", "Testing", "Deployment" }
                    },

                    new Question
                    {
                        QuestionTitle = "What is the primary tool in Kanban?",
                        Answer = "Kanban Board",
                        PossibleOptions = new List<string>
                            { "Task List", "Kanban Board", "Scrum Board", "Time Tracker" }
                    }
                ]
            }
        }
    };

    public TopicContent GetContent(string topic)
    {
        return _topics.TryGetValue(topic.ToUpper(), out var content) ? content : new TopicContent();
    }
}