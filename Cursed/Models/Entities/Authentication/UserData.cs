using System;
using System.Collections.Generic;

namespace Cursed.Models.Entities.Authentication
{
    public partial class UserData
    {
        public string Login { get; set; }
        public string RoleName { get; set; }

        public virtual UserAuth LoginNavigation { get; set; }
        public virtual Role Role { get; set; }
    }
}
