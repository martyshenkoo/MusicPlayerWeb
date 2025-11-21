namespace MusicPlayerWeb.Services.Commands;

public class StopTrackCommand : IPlayerCommand
{
    private readonly PlayerStateService _playerState;

    public string Name => "Stop";

    public StopTrackCommand(PlayerStateService playerState)
    {
        _playerState = playerState;
    }

    public void Execute()
    {
        _playerState.Stop();
    }
}
