using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicPlayerWeb.Services;
using MusicPlayerWeb.Services.Playlists;

namespace MusicPlayerWeb.Controllers;

[Authorize]
public class TracksController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly ITrackService _tracks;
    private readonly IPlaylistService _playlists;
    private readonly IPlaylistHistoryService _history;

    public TracksController(IWebHostEnvironment env, ITrackService tracks, IPlaylistService playlists, IPlaylistHistoryService history)
    {
        _env = env;
        _tracks = tracks;
        _playlists = playlists;
        _history = history;
    }

    [HttpPost]
    public async Task<IActionResult> Add(string title, IFormFile file, Guid playlistId)
    {
        var username = User.Identity!.Name!;

        if (playlistId == Guid.Empty)
        {
            TempData["Error"] = "Вибери плейліст.";
            return RedirectToAction("Index", "Home");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            TempData["Error"] = "Назва обовʼязкова.";
            return RedirectToAction("Index", "Home", new { playlistId });
        }

        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Файл не вибраний.";
            return RedirectToAction("Index", "Home", new { playlistId });
        }

        if (!_history.Backup(username, playlistId))
        {
            TempData["Error"] = "Плейліст не знайдено.";
            return RedirectToAction("Index", "Home", new { playlistId });
        }

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
            TempData["Error"] = "Не вдалося додати трек до плейліста.";
        }

        return RedirectToAction("Index", "Home", new { playlistId });
    }

    [HttpPost]
    public IActionResult Delete(Guid id, Guid playlistId)
    {
        var username = User.Identity!.Name!;
        if (!_history.Backup(username, playlistId))
        {
            TempData["Error"] = "Плейліст не знайдено.";
            return RedirectToAction("Index", "Home", new { playlistId });
        }

        var removed = _playlists.RemoveTrack(username, playlistId, id);
        if (!removed)
            TempData["Error"] = "Трек не знайдено в плейлісті.";

        return RedirectToAction("Index", "Home", new { playlistId });
    }
}
