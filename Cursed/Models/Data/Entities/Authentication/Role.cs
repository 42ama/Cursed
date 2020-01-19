using System;
using System.Collections.Generic;

namespace Cursed.Models.Entities.Authentication
{
    public partial class Role
    {
        public Role()
        {
            UserData = new HashSet<UserData>();
        }

        public string Name { get; set; }

        public virtual ICollection<UserData> UserData { get; set; }
    }
}
