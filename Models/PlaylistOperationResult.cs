using System;

namespace MusicPlayerWeb.Models;

public class PlaylistOperationResult
{
    public PlaylistOperationResult(bool success, Guid playlistId, string? message = null)
    {
        Success = success;
        PlaylistId = playlistId;
        Message = message;
    }

    public bool Success { get; }
    public Guid PlaylistId { get; }
    public string? Message { get; }

    public static PlaylistOperationResult SuccessResult(Guid playlistId, string? message = null)
        => new PlaylistOperationResult(true, playlistId, message);

    public static PlaylistOperationResult Failure(Guid playlistId, string message)
        => new PlaylistOperationResult(false, playlistId, message);
}
