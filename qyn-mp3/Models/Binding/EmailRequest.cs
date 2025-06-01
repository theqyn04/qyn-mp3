using System.ComponentModel.DataAnnotations;

namespace qyn_mp3.Models.Binding
{
    public class EmailRequest
    {
        [Required(ErrorMessage = "Email are required.")]
        [EmailAddress(ErrorMessage = "Email must be format are example@domain.com.")]
        public string Email { get; set; }
    }

}
