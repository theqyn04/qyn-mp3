using System.ComponentModel.DataAnnotations;

namespace qyn_mp3.Models.ViewModels
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "Input your username.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Input your full name.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Input your email.")]
        [EmailAddress(ErrorMessage = "Email are not accepted.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Input your password.")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password must be at least {2} characters.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Password must contain at least 1 uppercase letter, 1 lowercase letter, 1 number and 1 special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "OTP are required.")]
        public string Otp { get; set; }
    }

}
