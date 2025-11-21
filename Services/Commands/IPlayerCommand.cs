namespace MusicPlayerWeb.Services.Commands;

public interface IPlayerCommand
{
    string Name { get; }
    void Execute();
}
