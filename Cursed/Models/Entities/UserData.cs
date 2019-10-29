using System;
using System.Collections.Generic;

namespace Cursed.Models.Entities
{
    public partial class UserData
    {
        public string Login { get; set; }
        public int RoleId { get; set; }

        public virtual UserAuth LoginNavigation { get; set; }
        public virtual Role Role { get; set; }
    }
}
