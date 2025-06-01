using System.ComponentModel.DataAnnotations;

namespace qyn_mp3.Models.ViewModels
{
    public class VerifyOtpViewModel
    {
        [Required(ErrorMessage = "Input your email.")]
        [EmailAddress(ErrorMessage = "Email not accepted.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Input your Otp.")]
        [StringLength(6, MinimumLength = 6)]
        public string Otp { get; set; }
    }

}
