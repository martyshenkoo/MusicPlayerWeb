using MusicPlayerWeb.Models;

namespace MusicPlayerWeb.Services.Commands;

public class PlayerCommandInvoker
{
    private const int MaxHistory = 20;
    private readonly List<PlayerCommandHistoryItem> _history = new();

    public IReadOnlyCollection<PlayerCommandHistoryItem> History
    {
        get
        {
            lock (_history)
            {
                return _history.ToArray();
            }
        }
    }

    public void Execute(IPlayerCommand command, string username)
    {
        command.Execute();
        AddToHistory(command.Name, username);
    }

    private void AddToHistory(string commandName, string username)
    {
        lock (_history)
        {
            _history.Add(new PlayerCommandHistoryItem
            {
                CommandName = commandName,
                ExecutedBy = username,
                ExecutedAtUtc = DateTime.UtcNow
            });

            if (_history.Count > MaxHistory)
            {
                _history.RemoveRange(0, _history.Count - MaxHistory);
            }
        }
    }
}
