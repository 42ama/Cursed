using System.ComponentModel.DataAnnotations;

namespace Cursed.Models.DataModel.Authentication
{
    /// <summary>
    /// Model used for authentication of user
    /// </summary>
    public class LoginModel
    {
        [Required(ErrorMessage = "Login required")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
