using Dissertation.Models.Learn;
using Dissertation.Services.Interfaces;

namespace Dissertation.Services;

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
                    "At its heart, Scrum breaks down work into time-boxed iterations called Sprints, which usually last between two to four weeks. Each Sprint results in a potentially shippable product increment — meaning something real, testable, and valuable. This frequent delivery encourages constant feedback and learning.",
                    "Scrum defines three key roles to keep the process smooth and focused:",
                    "- Product Owner: Represents the voice of the customer. They define the vision, manage the product backlog, and ensure the team is working on what brings the most value.",
                    "- Scrum Master: The team's coach. They guide the team in following Scrum principles, remove blockers, and help the team stay productive and focused.",
                    "- Development Team: A cross-functional group of professionals who collaborate to design, build, and test the product. Everyone on the team is equally accountable for delivery.",
                    "Scrum is structured around ceremonies — regular events that keep everyone aligned and moving forward:",
                    "- Sprint Planning: The team selects which backlog items they will complete in the Sprint and define a goal.",
                    "- Daily Scrum (Stand-up): A quick daily meeting to synchronise progress and address any blockers.",
                    "- Sprint Review: The team demonstrates what they've built, and feedback is gathered from stakeholders.",
                    "- Sprint Retrospective: A reflection meeting where the team discusses what went well, what didn’t, and how to improve next time.",
                    "Scrum promotes transparency, inspection, and adaptation. Everyone can see the work, progress is reviewed frequently, and the plan is adjusted accordingly — this keeps the team responsive, not rigid.",
                    "Beyond software, Scrum has proven successful in industries such as marketing, education, product design, and more. Its emphasis on teamwork, fast feedback, and flexibility makes it a go-to choice for any team navigating complexity."
                ],

                SummaryPoints =
                [
                    "Scrum is an Agile framework built around short, iterative cycles called Sprints.",
                    "Key roles: Product Owner, Scrum Master, and Development Team.",
                    "Ceremonies include Sprint Planning, Daily Stand-up, Sprint Review, and Retrospective.",
                    "Focuses on transparency, inspection, and adaptation.",
                    "Encourages continuous feedback and delivery."
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
                    "Kanban is built on the principle of visualising work. Teams create a board with columns like ‘To Do’, ‘In Progress’, and ‘Done’, allowing everyone to see where each task is in its journey. This simple method reveals bottlenecks and helps teams stay organised.",
                    "A core idea in Kanban is to limit work in progress (WIP). By capping how many tasks are being worked on at once, teams avoid overload, improve focus, and deliver higher-quality outcomes faster.",
                    "Kanban encourages evolutionary change. Rather than overhauling everything at once, it promotes small, gradual improvements. Teams regularly review the flow of work to find ways to increase efficiency and reduce delays.",
                    "Originating in Toyota’s manufacturing system, Kanban has grown beyond its roots. It’s now used in software, healthcare, education, marketing — essentially, anywhere tasks need to move from idea to completion with clarity and flow."
                ],

                SummaryPoints =
                [
                    "Kanban uses a visual board to show work and its progress.",
                    "Work moves through columns like To Do, In Progress, and Done.",
                    "Limits the amount of work in progress (WIP) to avoid overload.",
                    "Supports continuous delivery without fixed sprints.",
                    "Helps teams improve flow and reduce delays."
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
                ],
                SummaryPoints =
                [
                    "Planning sets clear goals, timelines, and tasks for the project.",
                    "Helps the team stay on track and use resources well.",
                    "Includes estimating time, budget, and developer capacity.",
                    "Should consider risks, scope, and changing needs.",
                    "Strong planning leads to smoother execution."
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
                ],

                SummaryPoints =
                [
                    "Execution is when the planned work is carried out.",
                    "Tasks are assigned, started, and tracked for progress.",
                    "Regular updates help catch issues early.",
                    "Focuses on teamwork, quality, and meeting goals.",
                    "Adapts when changes or blockers appear."
                ]
            }
        },


        {
            "GLOSSARY",
            new TopicContent
            {
                TopicName = "Agile Glossary",
                Description =
                    "This glossary explains common terms used in Agile project management using clear, simple language. It's ideal for students learning how modern teams plan, build, and deliver software projects.",

                Paragraphs =
                [
                    "- Agile: A flexible way of managing projects that focuses on small steps, regular feedback, and constant improvement.\n  Example: Instead of planning the whole thing at once, the team builds and tests in short cycles.",

                    "- Scrum: A popular Agile method where teams work in short timeframes (called Sprints) to complete tasks and deliver value.\n  Example: A team delivers a working feature every two weeks.",

                    "- Sprint: A short period (usually 1–4 weeks) where a team works to finish a specific set of tasks.\n  Example: “We’ll finish the login system in this 2-week sprint.”",

                    "- User Story: A simple statement describing what a user wants and why, often written from the user’s point of view.\n  Example: “As a user, I want to reset my password so I don’t get locked out.”",

                    "- Product Owner: The person who decides what features the product needs and the order they should be done in.\n  Example: They choose which stories are most important for the next sprint.",

                    "- Scrum Master: A helper who makes sure the team follows Scrum rules and removes any blockers.\n  Example: If a developer is stuck waiting on something, the Scrum Master helps sort it out.",

                    "- Daily Stand-up: A quick meeting (usually 15 minutes) where team members say what they did, what they’ll do, and if they have any issues.\n  Example: “Yesterday I fixed a bug; today I’ll start the payment form.”",

                    "- Sprint Planning: A meeting at the start of the sprint where the team chooses which work to do.\n  Example: “Let’s pick 5 stories to complete in this sprint.”",

                    "- Sprint Review: A meeting at the end of a sprint to show what the team built and get feedback.\n  Example: “Here’s the new homepage—does this match what you expected?”",

                    "- Sprint Retrospective: A team discussion about what went well, what didn’t, and how to improve for next time.\n  Example: “Let’s try using clearer task names going forward.”",

                    "- Backlog: A list of everything that could be worked on in the future, like features, fixes, or improvements.\n  Example: The team picks tasks from the backlog during sprint planning.",

                    "- Burndown Chart: A simple graph that shows how much work is left to do in the sprint.\n  Example: If the line is flat, the team might be stuck.",

                    "- Velocity: The average amount of work a team can finish in one sprint.\n  Example: “We usually complete about 20 points of work each sprint.”",

                    "- Increment: A working part of the product delivered at the end of the sprint. It should be usable and testable.\n  Example: A finished shopping cart feature is an increment."
                ]
            }
        }
    };


    public TopicContent GetContent(string topic)
    {
        return _topics.TryGetValue(topic.ToUpper(), out var content) ? content : new TopicContent();
    }
}