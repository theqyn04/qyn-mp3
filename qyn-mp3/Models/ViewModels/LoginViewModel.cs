using System.ComponentModel.DataAnnotations;

namespace qyn_mp3.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Input your username.")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Input your password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; } // Cho phép null

    }
}
