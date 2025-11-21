using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicPlayerWeb.Models;
using MusicPlayerWeb.Services;
using MusicPlayerWeb.Services.Commands;

namespace MusicPlayerWeb.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ITrackService _tracks;
    private readonly IPlaylistService _playlists;
    private readonly PlayerStateService _playerState;
    private readonly PlayerCommandInvoker _commandInvoker;

    public HomeController(
        ITrackService tracks,
        IPlaylistService playlists,
        PlayerStateService playerState,
        PlayerCommandInvoker commandInvoker)
    {
        _tracks = tracks;
        _playlists = playlists;
        _playerState = playerState;
        _commandInvoker = commandInvoker;
    }

    public IActionResult Index(Guid? playlistId = null)
    {
        var username = User.Identity!.Name!;
        var playlists = _playlists.GetForUser(username).ToList();
        if (!playlistId.HasValue && playlists.Any())
            playlistId = playlists.First().Id;

        Playlist? selected = null;
        IEnumerable<Track> tracks = Enumerable.Empty<Track>();

        if (playlistId.HasValue)
        {
            selected = _playlists.GetById(username, playlistId.Value);
            if (selected != null)
                tracks = _playlists.GetTracks(username, playlistId.Value);
        }

        var model = new PlaylistPageViewModel
        {
            Playlists = playlists,
            SelectedPlaylistId = playlistId,
            SelectedPlaylistTitle = selected?.Title,
            Tracks = tracks,
            PlayerState = _playerState.Snapshot,
            CommandHistory = _commandInvoker.History
        };

        return View(model);
    }
}
