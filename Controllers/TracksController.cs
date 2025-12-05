using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MusicPlayerWeb.Services;

namespace MusicPlayerWeb.Controllers;

[Authorize]
public class TracksController : Controller
{
    private readonly PlayerFacade _facade;

    public TracksController(PlayerFacade facade)
    {
        _facade = facade;
    }

    [HttpPost]
    public async Task<IActionResult> Add(string title, IFormFile file, Guid playlistId)
    {
        var username = User.Identity!.Name!;
        var result = await _facade.AddTrackAsync(username, playlistId, title, file);
        if (!result.Success)
        {
            TempData["Error"] = result.Message;
        }
        else if (!string.IsNullOrEmpty(result.Message))
        {
            TempData["Info"] = result.Message;
        }

        return RedirectToAction("Index", "Home", new { playlistId = result.PlaylistId == Guid.Empty ? playlistId : result.PlaylistId });
    }

    [HttpPost]
    public IActionResult Delete(Guid id, Guid playlistId)
    {
        var username = User.Identity!.Name!;
        var result = _facade.RemoveTrack(username, playlistId, id);
        if (!result.Success)
            TempData["Error"] = result.Message;
        else if (!string.IsNullOrEmpty(result.Message))
            TempData["Info"] = result.Message;

        return RedirectToAction("Index", "Home", new { playlistId });
    }
}
