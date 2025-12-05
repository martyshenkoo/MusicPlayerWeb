using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicPlayerWeb.Models;
using MusicPlayerWeb.Services.ClientServer;

namespace MusicPlayerWeb.Controllers;

[Authorize]
[Route("[controller]/[action]")]
public class PlayerController : Controller
{
    private readonly PlayerClient _client;

    public PlayerController(PlayerClient client)
    {
        _client = client;
    }

    [HttpPost]
    public IActionResult Play([FromBody] PlayTrackCommandRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Url))
            return BadRequest("Недостатньо даних для програвання треку.");

        var username = User.Identity?.Name ?? "unknown";
        var result = _client.RequestPlay(username, request.Title.Trim(), request.Url);
        if (!result.Success)
            return BadRequest(result.Message);
        return Json(new { state = result.State, history = result.History, message = result.Message });
    }

    [HttpPost]
    public IActionResult Pause()
    {
        var username = User.Identity?.Name ?? "unknown";
        var result = _client.RequestPause(username);
        return Json(new { state = result.State, history = result.History, message = result.Message });
    }

    [HttpPost]
    public IActionResult Stop()
    {
        var username = User.Identity?.Name ?? "unknown";
        var result = _client.RequestStop(username);
        return Json(new { state = result.State, history = result.History, message = result.Message });
    }
}

public class PlayTrackCommandRequest
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
