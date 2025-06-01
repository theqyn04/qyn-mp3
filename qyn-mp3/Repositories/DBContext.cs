using Microsoft.EntityFrameworkCore;
using qyn_mp3.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace qyn_mp3.Repositories
{
    public class DBContext : IdentityDbContext<ApplicationUser>
    {
        public DBContext(DbContextOptions<DBContext> options)
        : base(options)
        {
        }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }
        public DbSet<ListeningHistory> ListeningHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure UserFavorite entity
            modelBuilder.Entity<UserFavorite>()
                .HasKey(uf => new { uf.UserId, uf.SongId }); // Composite primary key

            // Configure many-to-many relationship between ApplicationUser and Song via UserFavorite
            modelBuilder.Entity<UserFavorite>()
                .HasOne(uf => uf.User)
                .WithMany(u => u.FavoriteSongs)
                .HasForeignKey(uf => uf.UserId);

            modelBuilder.Entity<UserFavorite>()
                .HasOne(uf => uf.Song)
                .WithMany() // Song may not have a navigation property back to UserFavorite
                .HasForeignKey(uf => uf.SongId);
        }
    }
}
