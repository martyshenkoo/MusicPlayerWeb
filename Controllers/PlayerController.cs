using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicPlayerWeb.Services;
using MusicPlayerWeb.Services.Commands;

namespace MusicPlayerWeb.Controllers;

[Authorize]
[Route("[controller]/[action]")]
public class PlayerController : Controller
{
    private readonly PlayerCommandInvoker _invoker;
    private readonly PlayerStateService _state;

    public PlayerController(PlayerCommandInvoker invoker, PlayerStateService state)
    {
        _invoker = invoker;
        _state = state;
    }

    [HttpPost]
    public IActionResult Play([FromBody] PlayTrackCommandRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Url))
            return BadRequest("Недостатньо даних для програвання треку.");

        var username = User.Identity?.Name ?? "unknown";
        var command = new PlayTrackCommand(_state, request.Title.Trim(), request.Url);
        _invoker.Execute(command, username);
        return Json(new { state = _state.Snapshot, history = _invoker.History });
    }

    [HttpPost]
    public IActionResult Pause()
    {
        var username = User.Identity?.Name ?? "unknown";
        var command = new PauseTrackCommand(_state);
        _invoker.Execute(command, username);
        return Json(new { state = _state.Snapshot, history = _invoker.History });
    }

    [HttpPost]
    public IActionResult Stop()
    {
        var username = User.Identity?.Name ?? "unknown";
        var command = new StopTrackCommand(_state);
        _invoker.Execute(command, username);
        return Json(new { state = _state.Snapshot, history = _invoker.History });
    }
}

public class PlayTrackCommandRequest
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
