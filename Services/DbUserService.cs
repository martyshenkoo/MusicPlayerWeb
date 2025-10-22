using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MusicPlayerWeb.Data;
using MusicPlayerWeb.Models;

namespace MusicPlayerWeb.Services;

public class DbUserService : IUserService
{
    private readonly AppDbContext _db;

    public DbUserService(AppDbContext db) => _db = db;

    public bool Register(string username, string password, out string error)
    {
        error = "";

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            error = "Ім'я користувача і пароль обов'язкові.";
            return false;
        }

        username = username.Trim();

        if (_db.Users.Any(u => u.Username == username))
        {
            error = "Такий користувач вже існує.";
            return false;
        }

        var user = new User
        {
            Username = username,
            PasswordHash = Hash(password)
        };

        _db.Users.Add(user);
        _db.SaveChanges();
        return true;
    }

    public bool ValidateCredentials(string username, string password)
    {
        var hash = Hash(password);
        return _db.Users.AsNoTracking()
                        .Any(u => u.Username == username && u.PasswordHash == hash);
    }

    private static string Hash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }
}
