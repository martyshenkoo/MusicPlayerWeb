using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace MusicPlayerWeb.Controllers;

[Authorize]
public class SystemController : Controller
{
    private readonly IHostApplicationLifetime _lifetime;

    public SystemController(IHostApplicationLifetime lifetime) => _lifetime = lifetime;

    [HttpPost]
    public IActionResult Exit()
    {
        _lifetime.StopApplication();
        return Content("Завершення роботи...");
    }
}
