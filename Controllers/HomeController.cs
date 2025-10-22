using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicPlayerWeb.Services;

namespace MusicPlayerWeb.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ITrackService _tracks;

    public HomeController(ITrackService tracks) => _tracks = tracks;

    public IActionResult Index()
    {
        var username = User.Identity!.Name!;
        var model = _tracks.GetForUser(username);
        return View(model);
    }
}
