using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlayerVault.Data;
using PlayerVault.Models;
using System.Threading.Tasks;

namespace PlayerVault.Controllers
{
    public class PlayersController : Controller
    {
        private readonly PlayerVaultContext _context;

        public PlayersController(PlayerVaultContext context)
        {
            _context = context;
        }

        // GET: Players
        public async Task<IActionResult> Index(string sortOrder, string searchString, string playerPosition)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            // ViewData for Sorting & Filtering
            ViewData["NameSort"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["AgeSort"] = sortOrder == "Age" ? "age_desc" : "Age";
            ViewData["NationalitySort"] = sortOrder == "Nationality" ? "nationality_desc" : "Nationality";
            ViewData["MarketValueSort"] = sortOrder == "MarketValue" ? "value_desc" : "MarketValue";
            ViewData["PositionSort"] = sortOrder == "Position" ? "position_desc" : "Position";
            ViewData["CurrentFilter"] = searchString;

            // Approved Players
            var approvedPlayers = _context.Players
                .Where(p => p.UserId == userId && p.IsApproved)
                .AsQueryable();

            // Dropdown filter
            ViewData["PositionFilter"] = new SelectList(await _context.Players
                .Where(p => p.UserId == userId && p.IsApproved)
                .Select(p => p.Position)
                .Distinct()
                .ToListAsync());

            // Apply Search
            if (!String.IsNullOrEmpty(searchString))
            {
                approvedPlayers = approvedPlayers.Where(p =>
                    p.Name.Contains(searchString) ||
                    p.Nationality.Contains(searchString) ||
                    p.Position.Contains(searchString));
            }

            // Filter by Position
            if (!String.IsNullOrEmpty(playerPosition))
            {
                approvedPlayers = approvedPlayers.Where(p => p.Position == playerPosition);
            }

            // Sorting
            approvedPlayers = sortOrder switch
            {
                "name_desc" => approvedPlayers.OrderByDescending(p => p.Name),
                "Age" => approvedPlayers.OrderBy(p => p.Age),
                "age_desc" => approvedPlayers.OrderByDescending(p => p.Age),
                "Nationality" => approvedPlayers.OrderBy(p => p.Nationality),
                "nationality_desc" => approvedPlayers.OrderByDescending(p => p.Nationality),
                "MarketValue" => approvedPlayers.OrderBy(p => p.MarketValue),
                "value_desc" => approvedPlayers.OrderByDescending(p => p.MarketValue),
                "Position" => approvedPlayers.OrderBy(p => p.Position),
                "position_desc" => approvedPlayers.OrderByDescending(p => p.Position),
                _ => approvedPlayers.OrderBy(p => p.Name),
            };

            // 🆕 Add Pending Players (for collapsible)
            var pendingPlayers = await _context.Players
                .Where(p => p.UserId == userId && !p.IsApproved)
                .ToListAsync();

            ViewBag.PendingPlayers = pendingPlayers;

            return View(await approvedPlayers.AsNoTracking().ToListAsync());
        }




        // GET: Players/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Players/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Player player)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            player.UserId = userId.Value; // ✅ Correct assignment
            player.IsApproved = false;

            _context.Add(player);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // GET: Players/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var player = await _context.Players.FindAsync(id);
            if (player == null) return NotFound();
            return View(player);
        }

        // POST: Players/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Player player)
        {
            if (id != player.Id)
                return NotFound();

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var existingPlayer = await _context.Players.FindAsync(id);
            if (existingPlayer == null)
                return NotFound();

            if (existingPlayer.UserId != userId)
                return Unauthorized(); // 👮 prevent others from editing your data

            // Update only allowed fields
            existingPlayer.Name = player.Name;
            existingPlayer.Position = player.Position;
            existingPlayer.Nationality = player.Nationality;
            existingPlayer.Age = player.Age;
            existingPlayer.MarketValue = player.MarketValue;

            // Optional: mark as unapproved again after editing
            existingPlayer.IsApproved = false;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Players/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var player = await _context.Players.FindAsync(id);
            if (player == null) return NotFound();
            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var player = await _context.Players.FindAsync(id);
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Players/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var player = await _context.Players.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (player == null) return NotFound();
            return View(player);
        }
    }
}
