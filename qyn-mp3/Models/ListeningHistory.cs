using System.ComponentModel.DataAnnotations;

namespace qyn_mp3.Models
{
    public class ListeningHistory
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="UserId are required.")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        [Required(ErrorMessage = "SongId is required.")]
        public int SongId { get; set; }
        public virtual Song Song { get; set; }
        [Required(ErrorMessage = "ListenDate is required.")]
        public DateTime ListenDate { get; set; }
    }
}
