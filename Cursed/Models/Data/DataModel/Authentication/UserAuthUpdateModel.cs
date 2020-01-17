using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.DataModel.Authentication
{
    public class UserAuthUpdateModel
    {
        [Required(ErrorMessage = "Login required")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Old password required")]
        [DataType(DataType.Password)]
        public string PasswordOld { get; set; }

        [Required(ErrorMessage = "new password required")]
        [DataType(DataType.Password)]
        public string PasswordNew { get; set; }

        [Required(ErrorMessage = "Password conformation required")]
        [DataType(DataType.Password)]
        [Compare("PasswordNew")]
        public string PasswordNewConfirm { get; set; }
    }
}
