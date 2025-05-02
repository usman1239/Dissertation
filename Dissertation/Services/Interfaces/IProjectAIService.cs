using Dissertation.Models.Challenge.Enums;

namespace Dissertation.Services.Interfaces;

public interface IProjectAiService
{
    Task<string> GetProjectSuggestionAsync(AssistantMode assistantMode, string userQuery = "");
}