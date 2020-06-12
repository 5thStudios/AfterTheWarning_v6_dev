using System.ComponentModel.DataAnnotations;



namespace Models
{
    public class LoginModel
    {
        
        [Required(ErrorMessage = "*Login Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "*Invalid Login Email. Please re-enter.")]
        [Display(Name = "Login Email")]
        public string LoginId { get; set; }
        

        [Required(ErrorMessage = "*Password is required")]
        [StringLength(100, ErrorMessage = "*Password must be 6 or more characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

    }
}