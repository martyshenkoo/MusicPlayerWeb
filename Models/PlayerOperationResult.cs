using System.Collections.Generic;

namespace MusicPlayerWeb.Models;

public class PlayerOperationResult
{
    public PlayerOperationResult(PlayerStateSnapshot state, IReadOnlyCollection<PlayerCommandHistoryItem> history, bool success, string? message = null)
    {
        State = state;
        History = history;
        Success = success;
        Message = message;
    }

    public PlayerStateSnapshot State { get; }
    public IReadOnlyCollection<PlayerCommandHistoryItem> History { get; }
    public bool Success { get; }
    public string? Message { get; }
}
