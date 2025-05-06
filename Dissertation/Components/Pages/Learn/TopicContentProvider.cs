using Dissertation.Models.Interfaces;
using Dissertation.Models.Learn;

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
                Description =
                    "Scrum is a powerful yet lightweight Agile framework that enables teams to tackle complex challenges while delivering high-value products. Rooted in collaboration, adaptability, and rapid feedback, Scrum is ideal for teams looking to improve continuously and thrive in dynamic environments.",

                VideoPath = "https://www.youtube.com/embed/b02ZkndLk1Y",

                Paragraphs =
                [
                    "At its heart, Scrum breaks down work into timeboxed iterations called *Sprints*, which usually last between two to four weeks. Each Sprint results in a potentially shippable product increment — meaning something real, testable, and valuable. This frequent delivery encourages constant feedback and learning.",
                    "Scrum defines three key roles to keep the process smooth and focused:",
                    "- **Product Owner**: Represents the voice of the customer. They define the vision, manage the product backlog, and ensure the team is working on what brings the most value.",
                    "- **Scrum Master**: The team's coach. They guide the team in following Scrum principles, remove blockers, and help the team stay productive and focused.",
                    "- **Development Team**: A cross-functional group of professionals who collaborate to design, build, and test the product. Everyone on the team is equally accountable for delivery.",
                    "Scrum is structured around *ceremonies* — regular events that keep everyone aligned and moving forward:",
                    "- **Sprint Planning**: The team selects which backlog items they will complete in the Sprint and define a goal.",
                    "- **Daily Scrum (Stand-up)**: A quick daily meeting to synchronise progress and address any blockers.",
                    "- **Sprint Review**: The team demonstrates what they've built, and feedback is gathered from stakeholders.",
                    "- **Sprint Retrospective**: A reflection meeting where the team discusses what went well, what didn’t, and how to improve next time.",
                    "Scrum promotes *transparency*, *inspection*, and *adaptation*. Everyone can see the work, progress is reviewed frequently, and the plan is adjusted accordingly — this keeps the team responsive, not rigid.",
                    "Beyond software, Scrum has proven successful in industries such as marketing, education, product design, and more. Its emphasis on teamwork, fast feedback, and flexibility makes it a go-to choice for any team navigating complexity."
                ]
            }
        },
        {
            "KANBAN",
            new TopicContent
            {
                TopicName = "Kanban",
                Description =
                    "Kanban is a visual, flow-based approach to managing tasks and improving team performance. It helps teams focus on the most important work, reduce waste, and deliver results continuously and efficiently.",

                VideoPath = "https://www.youtube.com/embed/iVaFVa7HYj4",

                Paragraphs =
                [
                    "Kanban is built on the principle of *visualising work*. Teams create a board with columns like ‘To Do’, ‘In Progress’, and ‘Done’, allowing everyone to see where each task is in its journey. This simple method reveals bottlenecks and helps teams stay organised.",
                    "A core idea in Kanban is to *limit work in progress* (WIP). By capping how many tasks are being worked on at once, teams avoid overload, improve focus, and deliver higher-quality outcomes faster.",
                    "Kanban encourages *evolutionary change*. Rather than overhauling everything at once, it promotes small, gradual improvements. Teams regularly review the flow of work to find ways to increase efficiency and reduce delays.",
                    "Originating in Toyota’s manufacturing system, Kanban has grown beyond its roots. It’s now used in software, healthcare, education, marketing — essentially, anywhere tasks need to move from idea to completion with clarity and flow."
                ]
            }
        },
        {
            "PLANNING",
            new TopicContent
            {
                TopicName = "Planning",
                Description =
                    "Planning is a vital part of project management. It involves setting clear objectives, breaking them down into tasks, and ensuring the right resources are in place. Good planning keeps a project on track, within budget, and aligned with expectations.",

                VideoPath = "Videos/Planning.mp4",

                Paragraphs =
                [
                    "The planning phase sets the stage for everything that follows. It defines what success looks like, outlines who will do what, and establishes timelines. Without a solid plan, even great ideas can quickly fall apart.",
                    "Effective planning considers time, scope, budget, risks, and team capabilities. Milestones are identified, deadlines set, and dependencies clarified. The plan acts like a map — guiding the team through the twists and turns of delivery.",
                    "Planning isn’t just a one-time event. As projects progress, conditions change. Revisiting the plan regularly allows for adjustments and keeps the team aligned with evolving goals.",
                    "Risk management is a key aspect. Anticipating potential problems and preparing fallback strategies helps the team stay calm under pressure and respond proactively rather than reactively."
                ]
            }
        },
        {
            "EXECUTION",
            new TopicContent
            {
                TopicName = "Execution",
                Description =
                    "Execution is where the project plan turns into reality. It’s the phase where tasks are carried out, software is built, and progress is actively tracked. During execution, effective communication and regular adjustments are essential to keep things moving in the right direction.",

                VideoPath = "Videos/Execution.mp4",

                Paragraphs =
                [
                    "In this phase, the team implements the tasks defined during planning. This might involve coding, testing, designing, or releasing features. The project manager plays a key role in coordinating activities, tracking progress, and ensuring deadlines are met.",
                    "Modern development often uses Agile practices like Scrum or Kanban during execution. These methods help manage priorities and adjust quickly to changes. Daily check-ins and sprint reviews provide constant feedback loops.",
                    "Execution requires attention to quality. Continuous testing, code reviews, and integration practices ensure that problems are caught early — not at the end. Risk management is ongoing, and regular performance checks help prevent delays.",
                    "The team’s ability to adapt is crucial. If something isn’t working, plans can be revisited. Flexibility, clear communication, and teamwork are what drive successful execution."
                ]
            }
        }
    };

    public TopicContent GetContent(string topic)
    {
        return _topics.TryGetValue(topic.ToUpper(), out var content) ? content : new TopicContent();
    }
}