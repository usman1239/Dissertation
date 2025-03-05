using System.Security.Claims;
using Dissertation.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace Dissertation.Services;

public class UserService(AuthenticationStateProvider authenticationStateProvider) : IUserService
{
    private string? _userId;

    public async Task<string?> GetUserIdAsync()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity is not { IsAuthenticated: true })
            return null;

        _userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return _userId;
    }
}