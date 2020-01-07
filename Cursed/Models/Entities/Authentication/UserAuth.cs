using System;
using System.Collections.Generic;

namespace Cursed.Models.Entities.Authentication
{
    public partial class UserAuth
    {
        public string Login { get; set; }
        public string PasswordHash { get; set; }

        public virtual UserData UserData { get; set; }
    }
}
