using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MusicPlayerWeb.Services;

namespace MusicPlayerWeb.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _users;

    public AccountController(IUserService users) => _users = users;

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
    {
        if (_users.ValidateCredentials(username, password))
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return Redirect(returnUrl ?? "/");
        }

        ViewBag.Error = "Невірне ім'я користувача або пароль.";
        return View();
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public IActionResult Register(string username, string password)
    {
        if (_users.Register(username, password, out var err))
            return RedirectToAction(nameof(Login));

        ViewBag.Error = err;
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
}
