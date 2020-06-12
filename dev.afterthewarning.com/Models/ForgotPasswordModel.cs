using System.ComponentModel.DataAnnotations;


namespace Models
{
    public class ForgotPasswordModel 
    {

        [Required(ErrorMessage = "*Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "*Invalid email address. Please re-enter.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

    }
}