using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicPlayerWeb.Services;

namespace MusicPlayerWeb.Controllers;

[Authorize]
public class TracksController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly ITrackService _tracks;

    public TracksController(IWebHostEnvironment env, ITrackService tracks)
    {
        _env = env;
        _tracks = tracks;
    }

    [HttpPost]
    public async Task<IActionResult> Add(string? title, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Будь ласка, вибери аудіофайл.";
            return RedirectToAction("Index", "Home");
        }

        var uploads = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploads);

        var safeName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var path = Path.Combine(uploads, safeName);
        await using (var stream = System.IO.File.Create(path))
        {
            await file.CopyToAsync(stream);
        }
        var rel = $"/uploads/{safeName}";
        _tracks.AddForUser(User.Identity!.Name!, title ?? file.FileName, safeName, rel);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult Delete(Guid id)
    {
        var username = User.Identity!.Name!;
        if (_tracks.Delete(username, id))
        {
            // Тут можна додати фізичне видалення файлу, якщо зберігати шлях у моделі
        }
        return RedirectToAction("Index", "Home");
    }
}
