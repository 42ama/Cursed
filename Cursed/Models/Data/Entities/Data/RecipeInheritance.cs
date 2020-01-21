using System.ComponentModel.DataAnnotations;

namespace Cursed.Models.Entities.Data
{
    public partial class RecipeInheritance
    {
        [Required]
        public int ParentId { get; set; }
        [Required]
        public int ChildId { get; set; }

        public virtual Recipe Child { get; set; }
        public virtual Recipe Parent { get; set; }
    }
}
