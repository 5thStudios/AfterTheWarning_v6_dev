using System.ComponentModel.DataAnnotations;



namespace Models
{
    public class MembershipModel
    {
        [Required(ErrorMessage = "*First name is required")]
        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "*Last Initial is required")]
        [StringLength(1, ErrorMessage = "*1 Letter Only", MinimumLength = 1)]
        [DataType(DataType.Text)]
        [Display(Name = "Last Initial")]
        public string LastName { get; set; }





        [Required(ErrorMessage = "*Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "*Invalid email address. Please re-enter.")]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "*Invalid repeat email address. Please re-enter.")]
        [Display(Name = "Repeat Email")]
        [Compare("Email", ErrorMessage = "*Non-matching email address.")]
        public string ConfirmEmail { get; set; }





        [Required(ErrorMessage = "*Password is required")]
        [StringLength(100, ErrorMessage = "*Password must be 6 or more characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Re-enter Password")]
        [Compare("Password", ErrorMessage = "*Non-matching password.")]
        public string ConfirmPassword { get; set; }



        [Display(Name = "Valid Password")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "*Invalid Password")]
        public bool ValidPassword { get; set; }



        [Display(Name = "Receive Latest Update Notifications?")]
        public bool Subscribed { get; set; }



        public int memberId { get; set; }
    }
}