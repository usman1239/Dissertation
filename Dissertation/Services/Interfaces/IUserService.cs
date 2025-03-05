namespace Dissertation.Services.Interfaces;

public interface IUserService
{
    Task<string?> GetUserIdAsync();
}