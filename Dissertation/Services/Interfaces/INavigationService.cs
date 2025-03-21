namespace Dissertation.Services.Interfaces;

public interface INavigationService
{
    void NavigateTo(string uri, bool forceLoad = false);
}