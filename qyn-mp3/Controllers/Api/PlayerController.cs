using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using qyn_mp3.Models;
using qyn_mp3.Repositories;
using System.IO;

namespace qyn_mp3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IWebHostEnvironment _env;

        public PlayerController(DBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet("play/{songId}")]
        public IActionResult PlaySong(int songId)
        {
            var song = _context.Songs
                .Include(s => s.ArtistObj)
                .FirstOrDefault(s => s.Id == songId);

            if (song == null)
            {
                return NotFound(new { message = "Song not found" });
            }

            // Clean the file path - remove "song/" prefix if it exists
            var cleanFilePath = song.FilePath.StartsWith("song/")
                ? song.FilePath.Substring(5)
                : song.FilePath;

            var filePath = Path.Combine(_env.WebRootPath, "song", cleanFilePath);

            Console.WriteLine($"Final file path: {filePath}");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new
                {
                    message = "Audio file not found",
                    debugInfo = new
                    {
                        webRoot = _env.WebRootPath,
                        requestedFile = cleanFilePath,
                        fullPath = filePath
                    }
                });
            }

            return Ok(new
            {
                title = song.Title,
                artist = song.Artist ?? "Unknown Artist",
                coverArt = song.CoverArt,
                duration = song.Duration.ToString(@"m\:ss"),
                filePath = $"/song/{Uri.EscapeDataString(cleanFilePath)}"
            });
        }
    }
}