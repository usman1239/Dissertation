using Dissertation.Models.Challenge;
using Dissertation.Models.Challenge.Enums;

namespace Dissertation.Data;

public static class DbSeeder
{
    public static void SeedData(AppDbContext context)
    {
        if (context.Projects.Any()) return; // Prevent duplicate seeding
        SeedExampleProjects(context);
        SeedExampleUserStories(context);
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
                Budget = 5000,
                NumOfSprints = 5,
                DeveloperCosts = new Dictionary<DeveloperExperienceLevel, int>
                {
                    { DeveloperExperienceLevel.Junior, 200 },
                    { DeveloperExperienceLevel.MidLevel, 300 },
                    { DeveloperExperienceLevel.Senior, 400 }
                }
            },

            new()
            {
                Title = "Develop a Task Management Web App",
                Description =
                    "A startup needs a simple web app for tracking daily tasks, setting priorities, and managing deadlines. The application should allow users to create, update, and delete tasks, assign categories, and set reminders. A user-friendly dashboard with filtering and sorting options is required. The app should support multiple users with basic authentication and role-based access control. Additionally, a minimal analytics feature to track completed and pending tasks would be beneficial.",
                Budget = 15000,
                NumOfSprints = 5,
                DeveloperCosts = new Dictionary<DeveloperExperienceLevel, int>
                {
                    { DeveloperExperienceLevel.Junior, 800 },
                    { DeveloperExperienceLevel.MidLevel, 1000 },
                    { DeveloperExperienceLevel.Senior, 1200 }
                }
            },

            new()
            {
                Title = "Build an E-commerce Store for Handmade Crafts",
                Description =
                    "An artisan shop wants an online store to sell handmade goods, featuring a visually appealing and easy-to-navigate design. The platform should include product listings with images, descriptions, and pricing, along with a secure shopping cart and checkout system. Users should be able to create accounts, save favorite items, and track orders. The store should support multiple payment options, including credit cards and digital wallets. Additionally, an admin panel is required for managing inventory, processing orders, and handling customer inquiries. Basic SEO optimization and mobile responsiveness are also necessary.",
                Budget = 20000,
                NumOfSprints = 5,
                DeveloperCosts = new Dictionary<DeveloperExperienceLevel, int>
                {
                    { DeveloperExperienceLevel.Junior, 1000 },
                    { DeveloperExperienceLevel.MidLevel, 2000 },
                    { DeveloperExperienceLevel.Senior, 3000 }
                }
            }
        };

        context.Projects.AddRange(projects);
        context.SaveChanges();
    }

    private static void SeedExampleUserStories(AppDbContext context)
    {
        var projects = context.Projects.ToList();

        var userStories = new List<UserStory>();

        foreach (var project in projects) userStories.AddRange(GetUserStoriesForProject(project.Id, project.Title));

        context.UserStories.AddRange(userStories);
        context.SaveChanges();
    }

    private static List<UserStory> GetUserStoriesForProject(int projectId, string projectTitle)
    {
        return projectTitle switch
        {
            "Create a Landing Page for a Local Bakery" =>
            [
                new UserStory
                {
                    ProjectId = projectId, Title = "Design homepage layout",
                    Description = "Create a visually appealing homepage design",
                    StoryPoints = 3
                },
                new UserStory
                {
                    ProjectId = projectId, Title = "Implement mobile responsiveness",
                    Description = "Ensure the landing page works on all devices",
                    StoryPoints = 5
                },
                new UserStory
                {
                    ProjectId = projectId, Title = "Integrate contact form",
                    Description = "Allow customers to send inquiries",
                    StoryPoints = 3
                },
                new UserStory
                {
                    ProjectId = projectId, Title = "Optimize page loading speed",
                    Description = "Ensure the page loads in under 3 seconds",
                    StoryPoints = 8
                },
                new UserStory
                {
                    ProjectId = projectId, Title = "Add customer testimonials",
                    Description = "Showcase positive reviews from customers",
                    StoryPoints = 2
                },
                new UserStory
                {
                    ProjectId = projectId, Title = "Include a product gallery",
                    Description = "Display images of baked goods",
                    StoryPoints = 5
                },
                new UserStory
                {
                    ProjectId = projectId, Title = "SEO Optimization",
                    Description = "Ensure the site ranks well on Google",
                    StoryPoints = 8
                },
                new UserStory
                {
                    ProjectId = projectId, Title = "Add social media links",
                    Description = "Link to the bakery’s social media accounts",
                    StoryPoints = 2
                },
                new UserStory
                {
                    ProjectId = projectId, Title = "Implement analytics tracking",
                    Description = "Monitor site visits and user behavior",
                    StoryPoints = 6
                },
                new UserStory
                {
                    ProjectId = projectId, Title = "Deploy the website",
                    Description = "Host the website on a server",
                    StoryPoints = 4
                }
            ],

            "Develop a Task Management Web App" =>
            [
                new UserStory
                {
                    ProjectId = projectId, Title = "User authentication system",
                    Description = "Implement login & registration functionality",
                    StoryPoints = 8
                },

                new UserStory
                {
                    ProjectId = projectId, Title = "Create task model",
                    Description = "Define the structure of a task in the database",
                    StoryPoints = 5
                },

                new UserStory
                {
                    ProjectId = projectId, Title = "Task CRUD operations",
                    Description = "Allow users to create, read, update, delete tasks",
                    StoryPoints = 8
                },

                new UserStory
                {
                    ProjectId = projectId, Title = "Implement due date notifications",
                    Description = "Notify users when tasks are nearing their deadline",
                    StoryPoints = 6
                },

                new UserStory
                {
                    ProjectId = projectId, Title = "User roles and permissions",
                    Description = "Ensure only authorized users can edit tasks",
                    StoryPoints = 8
                },

                new UserStory
                {
                    ProjectId = projectId, Title = "Task filtering and sorting",
                    Description = "Allow users to filter tasks by status or priority",
                    StoryPoints = 5
                },

                new UserStory
                {
                    ProjectId = projectId, Title = "Add drag-and-drop task reordering",
                    Description = "Enable users to move tasks around easily",
                    StoryPoints = 7
                },

                new UserStory
                {
                    ProjectId = projectId, Title = "Implement task sharing",
                    Description = "Allow users to share tasks with teammates",
                    StoryPoints = 6
                },

                new UserStory
                {
                    ProjectId = projectId, Title = "Dark mode support",
                    Description = "Allow users to switch between light and dark themes",
                    StoryPoints = 4
                },

                new UserStory
                {
                    ProjectId = projectId, Title = "Deploy the web app",
                    Description = "Launch the application for public use",
                    StoryPoints = 5
                }
            ],

            "Build an E-commerce Store for Handmade Crafts" =>
            [
                new UserStory
                {
                    ProjectId = projectId, Title = "Design homepage layout",
                    Description = "Create a welcoming storefront for the website",
                    StoryPoints = 5
                },

                new UserStory
                {
                    ProjectId = projectId, Title = "Implement product catalog",
                    Description = "Display all available handmade crafts",
                    StoryPoints = 8
                },

                new UserStory
                {
                    ProjectId = projectId, Title = "Add shopping cart functionality",
                    Description = "Enable users to add and remove items",
                    StoryPoints = 7
                },

                new UserStory
                {
                    ProjectId = projectId, Title = "Set up payment processing",
                    Description = "Integrate payment gateway for purchases",
                    StoryPoints = 9
                },

                new UserStory
                {
                    ProjectId = projectId, Title = "Enable order tracking",
                    Description = "Allow customers to track their orders",
                    StoryPoints = 6
                },

                new UserStory
                {
                    ProjectId = projectId, Title = "User account management",
                    Description = "Allow users to create and manage accounts",
                    StoryPoints = 7
                },

                new UserStory
                {
                    ProjectId = projectId, Title = "Implement discount codes",
                    Description = "Allow shop owners to provide discounts",
                    StoryPoints = 5
                },

                new UserStory
                {
                    ProjectId = projectId, Title = "Add product reviews & ratings",
                    Description = "Enable customers to leave feedback",
                    StoryPoints = 6
                },

                new UserStory
                {
                    ProjectId = projectId, Title = "Optimize website for SEO",
                    Description = "Improve search engine visibility",
                    StoryPoints = 8
                },

                new UserStory
                {
                    ProjectId = projectId, Title = "Deploy the e-commerce site",
                    Description = "Launch the store online",
                    StoryPoints = 5
                }
            ],

            _ => []
        };
    }
}