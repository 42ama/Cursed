using System;
using System.Collections.Generic;

namespace Cursed.Models.Entities
{
    public partial class RecipeInheritance
    {
        public int ParentId { get; set; }
        public int ChildId { get; set; }

        public virtual Recipe Child { get; set; }
        public virtual Recipe Parent { get; set; }
    }
}
