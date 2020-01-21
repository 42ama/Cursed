using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cursed.Models.Entities.Data
{
    public partial class RecipeProductChanges
    {
        [Required]
        public int RecipeId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Quantity must be positive")]
        public decimal Quantity { get; set; }

        public virtual ProductCatalog Product { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
