using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PlayerVault.Data;
using PlayerVault.Models;
using PlayerVault.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using BCrypt.Net;

namespace PlayerVault.Controllers
{
    public class AuthController : Controller
    {
        private readonly PlayerVaultContext _context;

        public AuthController(PlayerVaultContext context)
        {
            _context = context;
        }

        // ------------------- LOGIN -------------------
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            if (!user.IsVerified)
            {
                ModelState.AddModelError("", "Account not verified by Admin.");
                return View(model);
            }

            // Save session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);

            // Redirect based on role
            if (user.Role == "Admin")
                return RedirectToAction("PendingPlayers", "Admin");

            return RedirectToAction("Index", "Players");
        }

        // ------------------- REGISTER -------------------
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var emailExists = await _context.Users.AnyAsync(u => u.Email == model.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "Email is already registered.");
                return View(model);
            }

            var usernameExists = await _context.Users.AnyAsync(u => u.Username == model.Username);
            if (usernameExists)
            {
                ModelState.AddModelError("Username", "Username is already taken.");
                return View(model);
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var newUser = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = hashedPassword,
                Role = "User",
                IsVerified = false
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Registration successful! Please wait for admin approval.";
            return RedirectToAction("Login");
        }

        // ------------------- LOGOUT -------------------
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
