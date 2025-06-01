using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using qyn_mp3.Repositories;

namespace qyn_mp3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DBContext _context;

        public HomeController(ILogger<HomeController> logger, DBContext dbContext )
        {
            _logger = logger;
            _context = dbContext;
        }

        public IActionResult Index()
        {
            var songs = _context.Songs.Take(5).ToList();
            return View(songs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        
    }
}
