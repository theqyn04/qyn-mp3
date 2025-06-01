using System.ComponentModel.DataAnnotations;

namespace qyn_mp3.Models
{
    public class Playlist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        [Required(ErrorMessage = "UserId is required.")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public string? Description { get; set; }
        public string? CoverImage { get; set; }
        public bool IsPublic { get; set; }

        // Navigation properties
        public virtual ICollection<Song> Songs { get; set; }
    }
}
