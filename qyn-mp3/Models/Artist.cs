using System.ComponentModel.DataAnnotations;

namespace qyn_mp3.Models
{
    public class Artist
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        public string? Bio { get; set; }
        public string? ImageUrl { get; set; }

        // Navigation properties
        public virtual ICollection<Song> Songs { get; set; }
        public virtual ICollection<Album> Albums { get; set; }
    }
}
