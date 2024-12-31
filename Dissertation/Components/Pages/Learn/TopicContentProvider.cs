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
                ]
            }
        },
        {
            "PLANNING",
            new TopicContent
            {
                TopicName = "Planning",
                Description =
                    "Planning is a crucial aspect of project management that involves defining goals, outlining tasks, and determining the necessary resources to achieve objectives. Effective planning ensures that projects are completed on time, within budget, and meet the desired outcomes. It requires careful consideration of potential risks, available resources, and stakeholder expectations.",
                Paragraphs =
                [
                    "In project management, planning is the first and most critical step in ensuring the success of a project. By clearly defining the objectives and breaking down the necessary tasks, a well-structured plan serves as a roadmap that guides the team through each stage of the project. It helps in identifying any potential obstacles or challenges early on, allowing for proactive solutions.",
                    "An effective plan takes into account the project scope, timeline, and available resources. It also involves identifying key milestones and setting realistic deadlines to keep the project on track. Additionally, planning requires collaboration among team members and stakeholders to ensure alignment with the overall vision and goals.",
                    "A key component of planning is risk management. By anticipating potential risks, project managers can develop mitigation strategies to minimise their impact. Regular review and adjustments to the plan throughout the project lifecycle are essential to ensure that the project stays aligned with its objectives."
                ],
                VideoPath = "Videos/Planning.mp4"
            }
        },

        {
            "EXECUTION",
            new TopicContent
            {
                TopicName = "Execution",
                Description =
                    "Execution in software development project management refers to the phase where the plans and strategies outlined during the planning phase are put into action. It involves coordinating teams, managing resources, and ensuring that tasks are completed efficiently and effectively to meet the project's objectives. During execution, continuous monitoring and adjustments are necessary to keep the project on track and ensure quality delivery within the specified timelines.",
                Paragraphs =
                [
                    "In the execution phase, the development team begins to implement the tasks defined during the planning phase. This includes writing code, conducting testing, and deploying features or updates. The project manager must ensure that resources are allocated properly and that any issues or obstacles are addressed swiftly. Effective communication and collaboration between team members, stakeholders, and other departments are key to keeping the project moving forward.",
                    "A crucial part of execution is managing the development cycle. This involves tracking progress against milestones, managing scope creep, and adjusting resources as necessary. Scrum or Kanban methodologies, for instance, can be utilised to manage tasks and sprints in an iterative manner, ensuring that the team remains focused on delivering small, incremental improvements over time.",
                    "Risk management and quality assurance play an integral role during execution. As development progresses, unforeseen challenges may arise. Continuous integration and testing help to identify issues early, preventing defects from accumulating. By monitoring performance, reviewing code regularly, and making necessary adjustments, teams can ensure that the final product meets both user requirements and project goals."
                ],
                VideoPath = "Videos/Execution.mp4"
            }
        }
    };

    public TopicContent GetContent(string topic)
    {
        return _topics.TryGetValue(topic.ToUpper(), out var content) ? content : new TopicContent();
    }
}