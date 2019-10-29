using System;
using System.Collections.Generic;

namespace Cursed.Models.Entities
{
    public partial class Role
    {
        public Role()
        {
            RoleHavePolicy = new HashSet<RoleHavePolicy>();
            UserData = new HashSet<UserData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<RoleHavePolicy> RoleHavePolicy { get; set; }
        public virtual ICollection<UserData> UserData { get; set; }
    }
}
