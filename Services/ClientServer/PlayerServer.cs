using MusicPlayerWeb.Models;

namespace MusicPlayerWeb.Services.ClientServer;

public class PlayerServer : IPlayerServer
{
    private readonly PlayerFacade _facade;

    public PlayerServer(PlayerFacade facade)
    {
        _facade = facade;
    }

    public PlayerOperationResult Play(string username, string title, string url)
        => _facade.PlayTrack(username, title, url);

    public PlayerOperationResult Pause(string username)
        => _facade.PauseTrack(username);

    public PlayerOperationResult Stop(string username)
        => _facade.StopTrack(username);
}
