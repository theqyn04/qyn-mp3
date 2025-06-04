using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using qyn_mp3.Models;
using qyn_mp3.Models.ViewModels;
using qyn_mp3.Repositories;
using System.Diagnostics;

namespace qyn_mp3.Controllers
{
    public class AlbumController : Controller
    {
        private readonly DBContext _context;
        private readonly ILogger<AlbumController> _logger;

        public AlbumController(DBContext context, ILogger<AlbumController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index(int id)
        {
            var album = _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Songs)
                .FirstOrDefault(a => a.Id == id);

            if (album == null)
            {
                return NotFound();
            }

            var viewModel = new AlbumViewModel
            {
                Album = album,
                RelatedAlbums = _context.Albums
                    .Where(a => a.ArtistId == album.ArtistId && a.Id != album.Id)
                    .Take(4)
                    .ToList()
            };

            return View(viewModel);
        }
    }
}