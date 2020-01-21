using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        [Required]
        public int Id { get; set; }
        [Required]
        public string Content { get; set; }
        public bool? TechApproval { get; set; }
        public bool? GovermentApproval { get; set; }

        public virtual ICollection<RecipeInheritance> RecipeInheritanceChild { get; set; }
        public virtual ICollection<RecipeInheritance> RecipeInheritanceParent { get; set; }
        public virtual ICollection<RecipeProductChanges> RecipeProductChanges { get; set; }
        public virtual ICollection<TechProcess> TechProcess { get; set; }
    }
}
