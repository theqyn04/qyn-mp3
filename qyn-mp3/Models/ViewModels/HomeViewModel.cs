namespace qyn_mp3.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Song> RecentSongs { get; set; }
        public List<Album> RecentAlbums { get; set; }
        public ApplicationUser CurrentUser { get; set; }
    }
}
