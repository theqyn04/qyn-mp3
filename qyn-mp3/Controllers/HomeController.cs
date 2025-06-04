using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using qyn_mp3.Models;
using qyn_mp3.Models.ViewModels;
using qyn_mp3.Repositories;

namespace qyn_mp3.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, DBContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = dbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User); // Lấy user hiện tại

            var viewModel = new HomeViewModel
            {
                RecentSongs = _context.Songs.Take(5).ToList(),
                RecentAlbums = _context.Albums
                    .Include(a => a.Artist)
                    .Include(a => a.Songs)
                    .OrderByDescending(a => a.ReleaseYear)
                    .Take(6)
                    .ToList(),
                CurrentUser = user // Thêm user vào ViewModel
            };

            return View(viewModel);
        }


    }
}
