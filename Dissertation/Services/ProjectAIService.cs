using System.Net;
using System.Text.Json;
using Dissertation.Models.Challenge;
using Dissertation.Models.Challenge.Enums;
using Dissertation.Services.Interfaces;

namespace Dissertation.Services;

public class ProjectAiService(ProjectStateService projectStateService, IConfiguration config) : IProjectAiService
{
    private readonly string _apiKey = config["OpenRouter:ApiKey"]!;

    public async Task<string> GetProjectSuggestionAsync(AssistantMode assistantMode, string userQuery = "")
    {
        try
        {
            var context = BuildProjectContext();

            var roleInstructions = GetRoleInstructions(assistantMode);

            var prompt = string.IsNullOrWhiteSpace(userQuery)
                ? $"""
                   You are a project management assistant operating in {assistantMode} mode.
                   {roleInstructions}

                   Here is the current project state:
                   {context}

                   Respond with 2–3 suggestions using this format:
                   - **Suggestion:** One-sentence actionable advice.
                   - **Reasoning:** Brief justification for the suggestion.

                   Use clear and concise language. Avoid any generic tips.

                   Make it specific to this project and its current state. 

                   Mention team members, developers, stories etc. in response for better context.

                   For example, a junior developer would not be as good to assign to a complex story as a senior developer.
                   """
                : $"""
                   You are a project management assistant operating in {assistantMode} mode.
                   {roleInstructions}

                   Here is the current project state:
                   {context}

                   User has asked the following question:
                   "{userQuery}"

                   Give a clear specific answer. 

                   Use clear and concise language. Avoid any generic tips.

                   Make it specific to this project and its current state. 

                   Mention team members, developers, stories etc. in response for better context.

                   For example, a junior developer would not be as good to assign to a complex story as a senior developer.
                   """;

            using var http = new HttpClient();
            http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            http.DefaultRequestHeaders.Add("HTTP-Referer", "https://localhost:7040");

            var request = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful AI assistant for project management." },
                    new { role = "user", content = prompt }
                }
            };

            var response = await http.PostAsJsonAsync("https://openrouter.ai/api/v1/chat/completions", request);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();

                return response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized =>
                        "Error: Unauthorized. Please check your API key or authentication settings.",
                    HttpStatusCode.TooManyRequests =>
                        "Error: Too many requests. You've reached the rate limit. Please try again later.",
                    HttpStatusCode.InternalServerError =>
                        "Error: The server encountered an issue. Please try again later.",
                    HttpStatusCode.BadRequest => "Error: Bad request. Please check your input parameters.",
                    HttpStatusCode.NotFound => "Error: The requested resource could not be found.",
                    _ =>
                        $"Error: Unable to fetch suggestion. Status Code: {response.StatusCode}. Details: {errorMessage}"
                };
            }

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var content = json.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            return content?.Trim() ?? "I'm not sure what to suggest right now.";
        }
        catch (Exception ex)
        {
            // Return a friendly error message if an exception occurs
            return $"Something went wrong while contacting the AI: {ex.Message}";
        }
    }

    private string BuildProjectContext()
    {
        var sprints = projectStateService.Sprints;
        var stories = projectStateService.UserStoryInstances;
        var budget = projectStateService.CurrentProjectInstance.Budget;
        var team = projectStateService.Team;
        var currentProject = projectStateService.CurrentProjectInstance.Project;

        // Budget
        var remainingBudget = budget > 0 ? $"£{budget:N0}" : "No budget left";

        // Incomplete user stories
        var incompleteStories = stories
            .Where(s => !s.IsComplete)
            .Select(s =>
            {
                var assigned = s.DeveloperAssigned != null
                    ? s.DeveloperAssigned.Name
                    : "Unassigned";
                return $"- \"{s.UserStory.Title}\" ({s.UserStory.StoryPoints} pts), assigned to: {assigned}";
            })
            .ToList();

        // Sprint performance: average completed stories per sprint
        var completedInPastSprints = sprints
            .Where(s => s.IsCompleted)
            .Select(s => s.ProjectInstance.UserStoryInstances.Count(us => us.IsComplete))
            .ToList();

        var averageStoryCompletion = completedInPastSprints.Count != 0
            ? completedInPastSprints.Average()
            : 0;

        // Team status: active and absent developers
        var absentDevs = team.Where(t => t.IsPermanentlyAbsent || t.IsSick).ToList();
        var activeDevs = team.Where(t => !t.IsPermanentlyAbsent && !t.IsSick).ToList();

        // Detailed team info
        var activeDevsInfo = activeDevs.Any()
            ? string.Join(", ", activeDevs.Select(t => $"{t.Name} ({t.ExperienceLevel})"))
            : "No active developers";
        var absentDevsInfo = absentDevs.Any()
            ? string.Join(", ", absentDevs.Select(t => t.Name))
            : "No absent developers";

        // Returning structured project context
        return $"""
                    **Project Overview**:
                    - {sprints.Count(s => s.IsCompleted)} of {currentProject.NumOfSprints} sprints completed.
                    - {stories.Count(s => s.IsComplete)} of {stories.Count} user stories completed.
                    - Remaining budget: {remainingBudget}.
                    - Average stories completed per sprint: {averageStoryCompletion:N2}
                    
                    **Team Info**:
                    - Active team: {activeDevsInfo}
                    - Absent team members: {absentDevsInfo}
                    
                    **Incomplete User Stories**:
                    {string.Join("\n", incompleteStories)}
                """;
    }


    private static string GetRoleInstructions(AssistantMode mode)
    {
        return mode switch
        {
            AssistantMode.Coach =>
                "Act like a supportive coach. Focus on motivation, balanced workloads, and developer well-being.",
            AssistantMode.Planner =>
                "Act like a strategic planner. Focus on future sprint planning, prioritization, and developer utilization.",
            AssistantMode.BudgetAnalyst =>
                "Act like a budget analyst. Focus on cost efficiency, developer cost distribution, and preventing budget overruns.",
            AssistantMode.Crisis =>
                "Act like a crisis manager. Focus on urgent issues, missed deadlines, and immediate actions to save the project.",
            _ =>
                "Give project improvement suggestions. You are to help the user improve in their understanding of software project management (mainly Scrum)"
        };
    }
}