using System;
using System.Collections.Generic;

namespace Cursed.Models.Entities.Authentication
{
    public partial class Policy
    {
        public Policy()
        {
            RoleHavePolicy = new HashSet<RoleHavePolicy>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<RoleHavePolicy> RoleHavePolicy { get; set; }
    }
}
