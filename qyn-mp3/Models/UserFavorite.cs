using System.ComponentModel.DataAnnotations;

namespace qyn_mp3.Models
{
    public class UserFavorite
    {
        [Required(ErrorMessage = "Id is required.")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        [Required(ErrorMessage = "SongId is required.")]
        public int SongId { get; set; }
        public virtual Song Song { get; set; }

        public DateTime AddedDate { get; set; }
    }
}
