using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicPlayerWeb.Services;

namespace MusicPlayerWeb.Controllers;

[Authorize]
public class TracksController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly ITrackService _tracks;
    private readonly IPlaylistService _playlists;

    public TracksController(IWebHostEnvironment env, ITrackService tracks, IPlaylistService playlists)
    {
        _env = env;
        _tracks = tracks;
        _playlists = playlists;
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

        _playlists.AddTrack(username, playlistId, track.Id);

        return RedirectToAction("Index", "Home", new { playlistId });
    }

    [HttpPost]
    public IActionResult Delete(Guid id, Guid playlistId)
    {
        var username = User.Identity!.Name!;
        _tracks.Delete(username, id);
        return RedirectToAction("Index", "Home", new { playlistId });
    }
}
