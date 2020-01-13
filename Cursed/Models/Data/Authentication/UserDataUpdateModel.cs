using Cursed.Models.Entities.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Data.Authentication
{
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
