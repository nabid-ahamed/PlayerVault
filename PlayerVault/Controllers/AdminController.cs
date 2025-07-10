using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlayerVault.Data;
using PlayerVault.Models;
using System.Linq;

public class AdminController : Controller
{
    private readonly PlayerVaultContext _context;

    public AdminController(PlayerVaultContext context)
    {
        _context = context;
    }

    // ✅ View users who registered but aren't verified
    public IActionResult PendingUsers()
    {
        var users = _context.Users
            .Where(u => !u.IsVerified && u.Role == "User")
            .ToList();

        return View(users);
    }

    // ✅ Approve user
    public IActionResult ApproveUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user != null)
        {
            user.IsVerified = true;
            _context.SaveChanges();
        }

        return RedirectToAction("PendingUsers");
    }

    // ✅ Reject (delete) user
    public IActionResult RejectUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        return RedirectToAction("PendingUsers");
    }

    // ✅ View unapproved players
    public IActionResult PendingPlayers()
    {
        var pending = _context.Players
            .Include(p => p.User) // make sure this is included
            .Where(p => !p.IsApproved)
            .ToList();

        return View(pending);
    }

    // ✅ Approve player
    public IActionResult Approve(int id)
    {
        var player = _context.Players.Find(id);
        if (player != null)
        {
            player.IsApproved = true;
            _context.SaveChanges();
        }

        return RedirectToAction("PendingPlayers");
    }

    // ✅ Reject (delete) player
    public IActionResult Reject(int id)
    {
        var player = _context.Players.Find(id);
        if (player != null)
        {
            _context.Players.Remove(player);
            _context.SaveChanges();
        }

        return RedirectToAction("PendingPlayers");
    }

    public IActionResult AllPlayers(int? userId)
    {
        var users = _context.Users
            .Where(u => u.IsVerified && u.Role == "User")
            .ToList();

        var playersQuery = _context.Players
            .Include(p => p.User)
            .Where(p => p.IsApproved);

        if (userId.HasValue)
        {
            playersQuery = playersQuery.Where(p => p.UserId == userId.Value);
        }

        ViewBag.Users = new SelectList(users, "Id", "Username", userId);
        ViewBag.SelectedUserId = userId;

        return View(playersQuery.ToList());
    }


}
