using System.ComponentModel.DataAnnotations;

namespace qyn_mp3.Models
{
    public class Song
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Artist { get; set; }
        [Required(ErrorMessage = "ArtistId is required.")]
        public int? ArtistId { get; set; }
        public virtual Artist ArtistObj { get; set; }

        [StringLength(255)]
        public string? Album { get; set; }

        public int? AlbumId { get; set; }
        public virtual Album AlbumObj { get; set; }

        public int? Year { get; set; }

        [StringLength(100)]
        public string? Genre { get; set; }

        [Required]
        public string FilePath { get; set; }

        public string? CoverArt { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        // Navigation properties
        public virtual ICollection<Playlist> Playlists { get; set; }
        public virtual ICollection<UserFavorite> UserFavorites { get; set; }
        public virtual ICollection<ListeningHistory> ListeningHistories { get; set; }
    }
}
