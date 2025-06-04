document.addEventListener('DOMContentLoaded', function () {
    // Xử lý click phát bài hát
    document.querySelectorAll('.song-row').forEach(row => {
        row.addEventListener('click', function (e) {
            // Chỉ phát nhạc khi không click vào nút play
            if (!e.target.closest('.play-button')) {
                const songId = this.getAttribute('data-song-id');
                playSong(songId);
            }
        });
    });

    // Xử lý nút play trên từng bài hát
    document.querySelectorAll('.song-row .play-button').forEach(button => {
        button.addEventListener('click', function (e) {
            e.stopPropagation();
            const songId = this.closest('.song-row').getAttribute('data-song-id');
            playSong(songId);
        });
    });

    // Xử lý nút play album
    document.querySelector('.album-actions .play-button').addEventListener('click', function () {
        const albumId = '@Model.Album.Id';
        playAlbum(albumId);
    });
});

function playSong(songId) {
    // Gọi API phát nhạc
    fetch(`/api/player/play/${songId}`)
        .then(response => response.json())
        .then(data => {
            updatePlayerBar(data);
        })
        .catch(error => console.error('Error:', error));
}

function playAlbum(albumId) {
    // Gọi API phát album
    fetch(`/api/player/play-album/${albumId}`)
        .then(response => response.json())
        .then(data => {
            updatePlayerBar(data.currentSong);
        })
        .catch(error => console.error('Error:', error));
}

function updatePlayerBar(song) {
    // Cập nhật player bar với thông tin bài hát
    const playerBar = document.querySelector('.player-bar');

    if (playerBar) {
        playerBar.querySelector('.song-title').textContent = song.title;
        playerBar.querySelector('.song-artist').textContent = song.artist;
        playerBar.querySelector('.song-cover').style.backgroundImage = `url(${song.coverArt})`;

        // Cập nhật thời lượng
        const durationElements = playerBar.querySelectorAll('.progress-time');
        if (durationElements.length > 1) {
            durationElements[1].textContent = song.duration;
        }
    }
}