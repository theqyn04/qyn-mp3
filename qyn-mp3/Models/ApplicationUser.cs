using Microsoft.AspNetCore.Identity;

namespace qyn_mp3.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? ProfileImage { get; set; } = "/css/images/avatar/avatar-default.png";


        // Navigation properties
        public virtual ICollection<Playlist> Playlists { get; set; }
        public virtual ICollection<UserFavorite> FavoriteSongs { get; set; }
        public virtual ICollection<ListeningHistory> ListeningHistories { get; set; }
    }
}
