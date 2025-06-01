using System.ComponentModel.DataAnnotations;

namespace qyn_mp3.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email are required.")]
        [EmailAddress(ErrorMessage = "Email not accepted.")]
        public string Email { get; set; }
    }

}
