using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerVault.Data;
using PlayerVault.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace PlayerVault.Controllers
{
    public class ProfileController : Controller
    {
        private readonly PlayerVaultContext _context;

        public ProfileController(PlayerVaultContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();

            var approvedCount = await _context.Players.CountAsync(p => p.UserId == userId && p.IsApproved);
            var pendingCount = await _context.Players.CountAsync(p => p.UserId == userId && !p.IsApproved);

            ViewBag.Username = user.Username;
            ViewBag.Email = user.Email;
            ViewBag.ApprovedCount = approvedCount;
            ViewBag.PendingCount = pendingCount;

            return View();
        }

        public async Task<IActionResult> Stats()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var players = await _context.Players
                .Where(p => p.UserId == userId)
                .ToListAsync();

            ViewBag.Total = players.Count;
            ViewBag.Approved = players.Count(p => p.IsApproved);
            ViewBag.Pending = players.Count(p => !p.IsApproved);
            ViewBag.AverageAge = players.Any() ? players.Average(p => p.Age) : 0;
            ViewBag.AverageValue = players.Any() ? players.Average(p => p.MarketValue) : 0;

            ViewBag.MostCommonPosition = players
                .GroupBy(p => p.Position)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            ViewBag.MostCommonNationality = players
                .GroupBy(p => p.Nationality)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            ViewBag.Youngest = players.OrderBy(p => p.Age).FirstOrDefault();
            ViewBag.Oldest = players.OrderByDescending(p => p.Age).FirstOrDefault();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            var viewModel = new EditProfileViewModel
            {
                Username = user.Username,
                Email = user.Email,
                CurrentEmail = user.Email
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Message"] = "Session expired. Please login again.";
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                // If validation fails, return the form so the user can see the errors.
                // Ensure your Edit.cshtml view has <span asp-validation-for="..."> helpers.
                return View(model);
            }

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
            {
                return NotFound();
            }

            // Update user properties from the model
            user.Username = model.Username;
            user.Email = model.Email;

            // Handle password change request
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                // --- Change Highlight ---
                // Added a check to ensure the current password was provided.
                if (string.IsNullOrEmpty(model.CurrentPassword) || !BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash))
                {
                    ModelState.AddModelError("CurrentPassword", "Current password must be provided and correct to set a new one.");
                    return View(model);
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            }

            // --- THE MAIN FIX ---
            // Explicitly tell Entity Framework to track this entity as modified.
            _context.Update(user);

            // Save all the tracked changes to the database.
            await _context.SaveChangesAsync();

            // Update session with new username if it changed
            HttpContext.Session.SetString("Username", user.Username);

            TempData["Message"] = "Profile updated successfully!";
            return RedirectToAction("Index");
        }
    }
}