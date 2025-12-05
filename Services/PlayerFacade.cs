using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MusicPlayerWeb.Models;
using MusicPlayerWeb.Services.Commands;
using MusicPlayerWeb.Services.Playlists;

namespace MusicPlayerWeb.Services;

public class PlayerFacade
{
    private readonly PlayerStateService _playerState;
    private readonly PlayerCommandInvoker _commandInvoker;
    private readonly ITrackService _tracks;
    private readonly IPlaylistService _playlists;
    private readonly IPlaylistHistoryService _history;
    private readonly IWebHostEnvironment _env;

    public PlayerFacade(
        PlayerStateService playerState,
        PlayerCommandInvoker commandInvoker,
        ITrackService tracks,
        IPlaylistService playlists,
        IPlaylistHistoryService history,
        IWebHostEnvironment env)
    {
        _playerState = playerState;
        _commandInvoker = commandInvoker;
        _tracks = tracks;
        _playlists = playlists;
        _history = history;
        _env = env;
    }

    public PlayerOperationResult PlayTrack(string username, string title, string url)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(url))
            return BuildPlayerResult(false, "Недостатньо даних для програвання треку.");

        var command = new PlayTrackCommand(_playerState, title.Trim(), url);
        _commandInvoker.Execute(command, username);

        return BuildPlayerResult(true, $"Відтворюється \"{title.Trim()}\".");
    }

    public PlayerOperationResult PauseTrack(string username)
    {
        var command = new PauseTrackCommand(_playerState);
        _commandInvoker.Execute(command, username);
        return BuildPlayerResult(true, "Трек призупинено.");
    }

    public PlayerOperationResult StopTrack(string username)
    {
        var command = new StopTrackCommand(_playerState);
        _commandInvoker.Execute(command, username);
        return BuildPlayerResult(true, "Відтворення зупинено.");
    }

    public async Task<PlaylistOperationResult> AddTrackAsync(string username, Guid playlistId, string title, IFormFile? file)
    {
        if (playlistId == Guid.Empty)
            return PlaylistOperationResult.Failure(Guid.Empty, "Вибери плейліст.");

        if (string.IsNullOrWhiteSpace(title))
            return PlaylistOperationResult.Failure(playlistId, "Назва треку є обовʼязковою.");

        if (file == null || file.Length == 0)
            return PlaylistOperationResult.Failure(playlistId, "Файл не вибраний.");

        if (!_history.Backup(username, playlistId))
            return PlaylistOperationResult.Failure(playlistId, "Плейліст не знайдено.");

        var uploads = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploads);

        var safe = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var path = Path.Combine(uploads, safe);

        await using (var stream = System.IO.File.Create(path))
        {
            await file.CopyToAsync(stream);
        }

        var rel = $"/uploads/{safe}";
        var track = _tracks.AddForUser(username, title, safe, rel);

        var added = _playlists.AddTrack(username, playlistId, track.Id);
        if (!added)
        {
            _tracks.Delete(username, track.Id);
            DeleteFileIfExists(path);
            return PlaylistOperationResult.Failure(playlistId, "Не вдалося додати трек до плейліста.");
        }

        return PlaylistOperationResult.SuccessResult(playlistId, "Трек додано до плейліста.");
    }

    public PlaylistOperationResult RemoveTrack(string username, Guid playlistId, Guid trackId)
    {
        if (playlistId == Guid.Empty)
            return PlaylistOperationResult.Failure(Guid.Empty, "Вибери плейліст.");

        if (!_history.Backup(username, playlistId))
            return PlaylistOperationResult.Failure(playlistId, "Плейліст не знайдено.");

        var removed = _playlists.RemoveTrack(username, playlistId, trackId);
        if (!removed)
            return PlaylistOperationResult.Failure(playlistId, "Трек не знайдено в плейлісті.");

        return PlaylistOperationResult.SuccessResult(playlistId, "Трек видалено з плейліста. Натисни \"Скасувати\", щоб повернути його.");
    }

    private PlayerOperationResult BuildPlayerResult(bool success, string? message = null)
    {
        return new PlayerOperationResult(_playerState.Snapshot, _commandInvoker.History, success, message);
    }

    private static void DeleteFileIfExists(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }
}
