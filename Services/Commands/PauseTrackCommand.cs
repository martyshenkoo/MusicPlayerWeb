namespace MusicPlayerWeb.Services.Commands;

public class PauseTrackCommand : IPlayerCommand
{
    private readonly PlayerStateService _playerState;

    public string Name => "Pause";

    public PauseTrackCommand(PlayerStateService playerState)
    {
        _playerState = playerState;
    }

    public void Execute()
    {
        _playerState.Pause();
    }
}
