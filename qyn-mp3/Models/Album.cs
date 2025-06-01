using System.ComponentModel.DataAnnotations;

namespace qyn_mp3.Models
{
    public class Album
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        public int? ArtistId { get; set; }
        public virtual Artist Artist { get; set; }
        public int? ReleaseYear { get; set; }
        public string? CoverArt { get; set; }

        // Navigation properties
        public virtual ICollection<Song> Songs { get; set; }

    }
}
