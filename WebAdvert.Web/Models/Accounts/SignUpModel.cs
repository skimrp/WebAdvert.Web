using System.ComponentModel.DataAnnotations;

namespace WebAdvert.Web.Models.Accounts
{
    public class SignUpModel
    {
        [Required]
        [EmailAddress]
        [Display(Name ="Email")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]       
        [Display(Name ="Password")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password is not matching!")]
       
        public string ConfirmPassword { get; set; }

    }
}
