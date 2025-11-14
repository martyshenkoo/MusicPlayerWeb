using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicPlayerWeb.Models;
using MusicPlayerWeb.Services;
using System.Linq;

namespace MusicPlayerWeb.Controllers;

[Authorize]
public class PlaylistsController : Controller
{
    private readonly IPlaylistService _playlists;

    public PlaylistsController(IPlaylistService playlists)
    {
        _playlists = playlists;
    }

    [HttpPost]
    public IActionResult Create(string title)
    {
        var username = User.Identity!.Name!;

        if (string.IsNullOrWhiteSpace(title))
        {
            TempData["Error"] = "Назва плейліста є обовʼязковою.";
            return RedirectToAction("Index", "Home");
        }

        var playlist = _playlists.Create(username, title);
        return RedirectToAction("Index", "Home", new { playlistId = playlist.Id });
    }

    [HttpPost]
    public IActionResult Rename(Guid playlistId, string newTitle)
    {
        var username = User.Identity!.Name!;

        if (string.IsNullOrWhiteSpace(newTitle))
        {
            TempData["Error"] = "Нова назва не може бути порожньою.";
            return RedirectToAction("Index", "Home", new { playlistId });
        }

        var ok = _playlists.Rename(username, playlistId, newTitle);

        if (!ok)
            TempData["Error"] = "Плейліст не знайдено.";

        return RedirectToAction("Index", "Home", new { playlistId });
    }

    [HttpPost]
    public IActionResult Delete(Guid playlistId)
    {
        var username = User.Identity!.Name!;

        var ok = _playlists.Delete(username, playlistId);

        if (!ok)
        {
            TempData["Error"] = "Плейліст не знайдено.";
            return RedirectToAction("Index", "Home");
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult View(Guid id)
    {
        var username = User.Identity!.Name!;
        var playlist = _playlists.GetById(username, id);

        if (playlist == null)
        {
            TempData["Error"] = "Плейліст не знайдено.";
            return RedirectToAction("Index", "Home");
        }

        var tracks = _playlists.GetTracks(username, id).ToList();

        ViewBag.Playlist = playlist;

        if (!tracks.Any())
        {
            ViewBag.HasIterator = false;
            return View(tracks);
        }
        var iterator = playlist.CreateIterator(tracks);

        ViewBag.HasIterator = true;
        ViewBag.FirstTrackTitle = iterator.Current().Title;
        ViewBag.TrackTitlesJson = System.Text.Json.JsonSerializer.Serialize(
            tracks.Select(t => t.Title).ToList()
        );

        return View(tracks);
    }
}
