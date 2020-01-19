using System.ComponentModel.DataAnnotations;

namespace Cursed.Models.DataModel.Authentication
{
    /// <summary>
    /// Model used for registration of new user
    /// </summary>
    public class RegistrationModel
    {
        [Required(ErrorMessage = "Login required")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password conformation required")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }

        [Required(ErrorMessage = "Role required")]
        public string RoleName { get; set; }
    }
}
