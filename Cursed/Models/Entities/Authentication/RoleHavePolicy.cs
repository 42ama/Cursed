using System;
using System.Collections.Generic;

namespace Cursed.Models.Entities.Authentication
{
    public partial class RoleHavePolicy
    {
        public int RoleId { get; set; }
        public int PolicyId { get; set; }

        public virtual Policy Policy { get; set; }
        public virtual Role Role { get; set; }
    }
}
