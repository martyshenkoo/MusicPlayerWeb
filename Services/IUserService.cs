using MusicPlayerWeb.Models;

namespace MusicPlayerWeb.Services;

public interface IUserService
{
    bool Register(string username, string password, out string error);
    bool ValidateCredentials(string username, string password);
}
