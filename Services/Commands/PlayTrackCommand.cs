namespace MusicPlayerWeb.Services.Commands;

public class PlayTrackCommand : IPlayerCommand
{
    private readonly PlayerStateService _playerState;
    private readonly string _title;
    private readonly string _url;

    public string Name => "Play";

    public PlayTrackCommand(PlayerStateService playerState, string title, string url)
    {
        _playerState = playerState;
        _title = title;
        _url = url;
    }

    public void Execute()
    {
        _playerState.Play(_title, _url);
    }
}
