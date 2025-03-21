using Dissertation.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Dissertation.Services;

public class NavigationService(NavigationManager navigationManager) : INavigationService
{
    public void NavigateTo(string uri, bool forceLoad = false)
    {
        navigationManager.NavigateTo(uri, forceLoad);
    }
}