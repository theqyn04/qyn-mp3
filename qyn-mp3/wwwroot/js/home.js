// Xử lý click nút play trên album
document.querySelectorAll('.play-button').forEach(button => {
    button.addEventListener('click', function (e) {
        e.preventDefault();
        e.stopPropagation();

        // Lấy albumId từ thẻ cha
        const albumId = this.closest('.album-card').getAttribute('data-album-id');
        playAlbum(albumId);
    });
});

// Xử lý click vào album card (chuyển đến trang album)
document.querySelectorAll('.album-card').forEach(card => {
    card.addEventListener('click', function (e) {
        // Chỉ chuyển trang nếu không click vào nút play
        if (!e.target.closest('.play-button')) {
            const albumId = this.getAttribute('data-album-id');
            window.location.href = `/Album/Index/${albumId}`;
        }
    });
});

function playAlbum(albumId) {
    // Gọi API để phát album
    fetch(`/api/player/play-album/${albumId}`)
        .then(response => response.json())
        .then(data => {
            // Cập nhật player bar
            updatePlayerBar(data.currentSong);
        });
}