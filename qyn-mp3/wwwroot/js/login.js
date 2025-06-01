document.addEventListener('DOMContentLoaded', function () {
    // Xử lý hiệu ứng focus cho các input
    const inputs = document.querySelectorAll('input');
    inputs.forEach(input => {
        input.addEventListener('focus', function () {
            this.parentElement.querySelector('label').style.color = '#1DB954';
        });

        input.addEventListener('blur', function () {
            this.parentElement.querySelector('label').style.color = '#FFFFFF';
        });
    });

    // Xử lý submit form
    const loginForm = document.querySelector('form');
    if (loginForm) {
        loginForm.addEventListener('submit', function (e) {
            const submitButton = this.querySelector('button[type="submit"]');
            if (submitButton) {
                submitButton.disabled = true;
                submitButton.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Loading...';
            }
        });
    }

    // Hiển thị thông báo lỗi từ TempData
    const errorMessage = @Html.Raw(JsonSerializer.Serialize(TempData["error"]));
    if (errorMessage) {
        showToast('Error', errorMessage, 'error');
    }

    const successMessage = @Html.Raw(JsonSerializer.Serialize(TempData["success"]));
    if (successMessage) {
        showToast('Success', successMessage, 'success');
    }

});

function showToast(title, message, type) {
    // Implement toast notification UI here
    // You can use libraries like Toastr or custom implementation
    console.log(`${title}: ${message} (${type})`);
    alert(`${title}: ${message}`);
}