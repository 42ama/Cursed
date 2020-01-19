using System;
using System.Collections.Generic;

namespace Cursed.Models.Entities.Data
{
    public partial class Recipe
    {
        public Recipe()
        {
            RecipeInheritanceChild = new HashSet<RecipeInheritance>();
            RecipeInheritanceParent = new HashSet<RecipeInheritance>();
            RecipeProductChanges = new HashSet<RecipeProductChanges>();
            TechProcess = new HashSet<TechProcess>();
        }

        public int Id { get; set; }
        public string Content { get; set; }
        public bool? TechApproval { get; set; }
        public bool? GovermentApproval { get; set; }

        public virtual ICollection<RecipeInheritance> RecipeInheritanceChild { get; set; }
        public virtual ICollection<RecipeInheritance> RecipeInheritanceParent { get; set; }
        public virtual ICollection<RecipeProductChanges> RecipeProductChanges { get; set; }
        public virtual ICollection<TechProcess> TechProcess { get; set; }
    }
}
