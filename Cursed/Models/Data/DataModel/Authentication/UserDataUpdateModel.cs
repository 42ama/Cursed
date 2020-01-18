using Cursed.Models.Entities.Authentication;
using System.ComponentModel.DataAnnotations;

namespace Cursed.Models.DataModel.Authentication
{
    /// <summary>
    /// Model used for updating User Data. Can be converted from UserData through constructor.
    /// </summary>
    public class UserDataUpdateModel
    {
        public UserDataUpdateModel() { }
        public UserDataUpdateModel(UserData userData)
        {
            Login = userData.Login;
            RoleName = userData.RoleName;
        }
        [Required(ErrorMessage = "Login required")]
        public string Login { get; set; }

        public string RoleName { get; set; }

        [Required(ErrorMessage = "Password required for conformation")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
