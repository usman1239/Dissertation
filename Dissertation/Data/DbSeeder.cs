using Dissertation.Models.Challenge;

namespace Dissertation.Data;

public static class DbSeeder
{
    public static void SeedData(AppDbContext context)
    {
        if (context.Projects.Any()) return;
        SeedExampleProjects(context);
    }

    private static void SeedExampleProjects(AppDbContext context)
    {
        var projects = new List<Project>
        {
            new()
            {
                Title = "Create a Landing Page for a Local Bakery",
                Description = 
                    "A small, family-owned bakery wants a professional online presence to attract more customers. You, as the project manager, must lead a small team to design, develop, and launch a responsive website within a limited budget and timeline.",
                Budget = 5000
            },
            
            new()
            {
                Title = "Develop a Task Management Web App",
                Description = "A startup needs a simple web app for tracking daily tasks...",
                Budget = 15000
            },

            new()
            {
                Title = "Build an E-commerce Store for Handmade Crafts",
                Description = "An artisan shop wants an online store to sell handmade goods...",
                Budget = 20000
            }
        };

        context.Projects.AddRange(projects);
        context.SaveChanges();
    }
}


//private static void SeedEcommerceProject(AppDbContext context)
//    {
//        var project = new Project
//        {
//            Name = "E-commerce Website Development",
//            Description = "Developing a scalable e-commerce platform using Scrum.",
//            Difficulty = 2
//        };
//        context.Projects.Add(project);
//        context.SaveChanges();

//        var scenarios = SeedECommerceScenarios(context, project);

//        SeedECommerceChoices(context, scenarios);
//    }

//private static void SeedECommerceChoices(AppDbContext context, List<Scenario> scenarios)
//{
//    var choices = new List<Choice>
//    {
//        // Planning Phase
//        new()
//        {
//            ScenarioId = scenarios[0].Id, ChoiceText = "Core shopping features (MVP)", IsCorrect = true,
//            NextScenarioId = scenarios[1].Id
//        },
//        new() { ScenarioId = scenarios[0].Id, ChoiceText = "Everything upfront", IsCorrect = false },
//        new() { ScenarioId = scenarios[0].Id, ChoiceText = "No backlog", IsCorrect = false },

//        new()
//        {
//            ScenarioId = scenarios[1].Id, ChoiceText = "Cross-functional Scrum team", IsCorrect = true,
//            NextScenarioId = scenarios[2].Id
//        },
//        new() { ScenarioId = scenarios[1].Id, ChoiceText = "Frontend only", IsCorrect = false },
//        new() { ScenarioId = scenarios[1].Id, ChoiceText = "One-person team", IsCorrect = false },

//        new()
//        {
//            ScenarioId = scenarios[2].Id, ChoiceText = "Database-driven dynamic listing", IsCorrect = true,
//            NextScenarioId = scenarios[3].Id
//        },
//        new() { ScenarioId = scenarios[2].Id, ChoiceText = "Hardcoded product list", IsCorrect = false },
//        new() { ScenarioId = scenarios[2].Id, ChoiceText = "Manual product updates", IsCorrect = false },

//        new()
//        {
//            ScenarioId = scenarios[3].Id, ChoiceText = "Use Stripe/PayPal", IsCorrect = true,
//            NextScenarioId = scenarios[4].Id
//        },
//        new() { ScenarioId = scenarios[3].Id, ChoiceText = "Store card details locally", IsCorrect = false },
//        new() { ScenarioId = scenarios[3].Id, ChoiceText = "Cash on delivery only", IsCorrect = false },

//        new()
//        {
//            ScenarioId = scenarios[4].Id, ChoiceText = "Show error & offer alternatives", IsCorrect = true,
//        },

//        // Development Phase
//        new()
//        {
//            ScenarioId = scenarios[5].Id, ChoiceText = "Use a single-page application", IsCorrect = true,
//            NextScenarioId = scenarios[6].Id
//        },
//        new()
//        {
//            ScenarioId = scenarios[5].Id, ChoiceText = "Separate product and payment pages", IsCorrect = false
//        },
//        new() { ScenarioId = scenarios[5].Id, ChoiceText = "Use hardcoded content", IsCorrect = false },

//        new()
//        {
//            ScenarioId = scenarios[6].Id, ChoiceText = "Stripe & PayPal", IsCorrect = true,
//            NextScenarioId = scenarios[7].Id
//        },
//        new() { ScenarioId = scenarios[6].Id, ChoiceText = "Bitcoin only", IsCorrect = false },
//        new() { ScenarioId = scenarios[6].Id, ChoiceText = "Local payment gateway", IsCorrect = false },

//        new()
//        {
//            ScenarioId = scenarios[7].Id, ChoiceText = "Use a microservices architecture", IsCorrect = true,
//            NextScenarioId = scenarios[8].Id
//        },
//        new() { ScenarioId = scenarios[7].Id, ChoiceText = "Monolithic architecture", IsCorrect = false },

//        new()
//        {
//            ScenarioId = scenarios[8].Id, ChoiceText = "Use a NoSQL database", IsCorrect = true,
//            NextScenarioId = scenarios[9].Id
//        },
//        new() { ScenarioId = scenarios[8].Id, ChoiceText = "Relational database", IsCorrect = false },

//        new() { ScenarioId = scenarios[9].Id, ChoiceText = "Use cloud storage", IsCorrect = true },

//        // Testing Phase
//        new()
//        {
//            ScenarioId = scenarios[10].Id, ChoiceText = "Use JMeter for load testing", IsCorrect = true,
//            NextScenarioId = scenarios[11].Id
//        },
//        new() { ScenarioId = scenarios[10].Id, ChoiceText = "Use Postman for testing", IsCorrect = false },

//        new()
//        {
//            ScenarioId = scenarios[11].Id, ChoiceText = "Use Selenium for UI testing", IsCorrect = true,
//            NextScenarioId = scenarios[12].Id
//        },
//        new() { ScenarioId = scenarios[11].Id, ChoiceText = "Use manual testing", IsCorrect = false },

//        new()
//        {
//            ScenarioId = scenarios[12].Id, ChoiceText = "Redirect users to an error page", IsCorrect = true,
//            NextScenarioId = scenarios[13].Id
//        },
//        new() { ScenarioId = scenarios[12].Id, ChoiceText = "Allow users to retry immediately", IsCorrect = false },

//        new()
//        {
//            ScenarioId = scenarios[13].Id, ChoiceText = "Test on multiple browsers", IsCorrect = true,
//            NextScenarioId = scenarios[14].Id
//        },
//        new() { ScenarioId = scenarios[13].Id, ChoiceText = "Only test on Chrome", IsCorrect = false },

//        new() { ScenarioId = scenarios[14].Id, ChoiceText = "Use penetration testing tools", IsCorrect = true },

//        // Maintenance Phase
//        new()
//        {
//            ScenarioId = scenarios[15].Id, ChoiceText = "Have a dedicated support team", IsCorrect = true,
//            NextScenarioId = scenarios[16].Id
//        },
//        new()
//        {
//            ScenarioId = scenarios[15].Id, ChoiceText = "Only rely on automated issue tracking", IsCorrect = false
//        },

//        new()
//        {
//            ScenarioId = scenarios[16].Id, ChoiceText = "Release regular updates", IsCorrect = true,
//            NextScenarioId = scenarios[17].Id
//        },
//        new()
//        {
//            ScenarioId = scenarios[16].Id, ChoiceText = "Wait for critical issues before updating",
//            IsCorrect = false
//        },

//        new()
//        {
//            ScenarioId = scenarios[17].Id, ChoiceText = "Use monitoring tools like New Relic", IsCorrect = true,
//            NextScenarioId = scenarios[18].Id
//        },
//        new() { ScenarioId = scenarios[17].Id, ChoiceText = "Only monitor logs manually", IsCorrect = false },

//        new()
//        {
//            ScenarioId = scenarios[18].Id, ChoiceText = "Use cloud scaling solutions", IsCorrect = true,
//            NextScenarioId = scenarios[19].Id
//        },
//        new() { ScenarioId = scenarios[18].Id, ChoiceText = "Use local servers only", IsCorrect = false },

//        new() { ScenarioId = scenarios[19].Id, ChoiceText = "Offer live chat support", IsCorrect = true },

//        // Deployment Phase
//        new()
//        {
//            ScenarioId = scenarios[20].Id, ChoiceText = "Create a deployment checklist", IsCorrect = true,
//            NextScenarioId = scenarios[21].Id
//        },
//        new() { ScenarioId = scenarios[20].Id, ChoiceText = "Deploy without testing", IsCorrect = false },

//        new()
//        {
//            ScenarioId = scenarios[21].Id, ChoiceText = "Test rollback procedures before deployment",
//            IsCorrect = true, NextScenarioId = scenarios[22].Id
//        },
//        new() { ScenarioId = scenarios[21].Id, ChoiceText = "Skip rollback testing", IsCorrect = false },

//        new()
//        {
//            ScenarioId = scenarios[22].Id, ChoiceText = "Use blue-green deployment", IsCorrect = true,
//            NextScenarioId = scenarios[23].Id
//        },
//        new() { ScenarioId = scenarios[22].Id, ChoiceText = "Deploy directly to production", IsCorrect = false },

//        new()
//        {
//            ScenarioId = scenarios[23].Id, ChoiceText = "Use automation for smooth handover", IsCorrect = true,
//            NextScenarioId = scenarios[24].Id
//        },
//        new()
//        {
//            ScenarioId = scenarios[23].Id, ChoiceText = "Leave handover to the operations team manually",
//            IsCorrect = false
//        },

//        new() { ScenarioId = scenarios[24].Id, ChoiceText = "Use monitoring tools like Datadog", IsCorrect = true },

//        // Feedback Phase
//        new()
//        {
//            ScenarioId = scenarios[25].Id, ChoiceText = "Use surveys to gather feedback", IsCorrect = true,
//            NextScenarioId = scenarios[26].Id
//        },
//        new() { ScenarioId = scenarios[25].Id, ChoiceText = "Wait for customers to reach out", IsCorrect = false },

//        new()
//        {
//            ScenarioId = scenarios[26].Id, ChoiceText = "Prioritize feedback based on business impact",
//            IsCorrect = true, NextScenarioId = scenarios[27].Id
//        },
//        new()
//        {
//            ScenarioId = scenarios[26].Id, ChoiceText = "Prioritize feedback from developers", IsCorrect = false
//        },

//        new()
//        {
//            ScenarioId = scenarios[27].Id, ChoiceText = "Use customer feedback tracking tools", IsCorrect = true,
//            NextScenarioId = scenarios[28].Id
//        },
//        new()
//        {
//            ScenarioId = scenarios[27].Id, ChoiceText = "Manually track feedback in spreadsheets", IsCorrect = false
//        },

//        new()
//        {
//            ScenarioId = scenarios[28].Id, ChoiceText = "Analyze feedback and prioritize quick wins",
//            IsCorrect = true, NextScenarioId = scenarios[29].Id
//        },
//        new()
//        {
//            ScenarioId = scenarios[28].Id, ChoiceText = "Ignore feedback and continue development",
//            IsCorrect = false
//        },

//        new()
//        {
//            ScenarioId = scenarios[29].Id, ChoiceText = "Have a dedicated feedback response team", IsCorrect = true
//        }
//    };
//    context.Choices.AddRange(choices);
//    context.SaveChanges();
//}

//private static List<Scenario> SeedECommerceScenarios(AppDbContext context, Project project)
//{
//    var scenarios = new List<Scenario>
//    {
//        // Planning Phase
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Planning,
//            Description = "What should be included in the initial product backlog?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Planning,
//            Description = "How should the first Scrum team be structured?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Planning,
//            Description = "How do you define the project's scope and goals?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Planning,
//            Description = "What are the initial steps to gather requirements?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Planning,
//            Description = "How will you prioritise the backlog items?"
//        },

//        // Development Phase
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Development,
//            Description = "How should the product listing page be built?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Development,
//            Description = "Which payment processing strategy is best?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Development, Description = "What technology stack will you use?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Development,
//            Description = "How should the architecture be structured?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Development,
//            Description = "How will you handle data persistence and storage?"
//        },

//        // Testing Phase
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Testing, Description = "How will you test for performance?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Testing,
//            Description = "What tools will you use for automated testing?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Testing, Description = "How should payment failures be handled?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Testing,
//            Description = "How do you ensure compatibility across devices?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Testing, Description = "What security testing will be conducted?"
//        },

//        // Maintenance Phase
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Maintenance,
//            Description = "How will you handle bug fixes after launch?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Maintenance,
//            Description = "How will you manage software updates?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Maintenance,
//            Description = "What is the process for monitoring system performance?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Maintenance, Description = "How will you handle scalability?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Maintenance,
//            Description = "What is your strategy for customer support?"
//        },

//        // Deployment Phase
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Deployment, Description = "How do you prepare for the deployment?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Deployment,
//            Description = "How will you handle rollback in case of issues?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Deployment,
//            Description = "What steps are required for a successful production deployment?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Deployment,
//            Description = "How do you ensure a smooth handover to operations?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Deployment,
//            Description = "What post-deployment monitoring is necessary?"
//        },

//        // Feedback Phase
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Feedback,
//            Description = "How do you gather feedback from stakeholders?"
//        },
//        new() { ProjectId = project.Id, Phase = Phase.Feedback, Description = "How will you prioritize feedback?" },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Feedback,
//            Description = "What tools will you use to track user feedback?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Feedback,
//            Description = "How will you analyze feedback for continuous improvement?"
//        },
//        new()
//        {
//            ProjectId = project.Id, Phase = Phase.Feedback,
//            Description = "What strategies do you use for addressing feedback?"
//        }
//    };
//    context.Scenarios.AddRange(scenarios);
//    context.SaveChanges();
//    return scenarios;
//}