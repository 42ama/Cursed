using System;
using System.Collections.Generic;

namespace Cursed.Models.Entities
{
    public partial class RecipeProductChanges
    {
        public int RecipeId { get; set; }
        public int ProductId { get; set; }
        public string Type { get; set; }
        public decimal Quantity { get; set; }

        public virtual ProductCatalog Product { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
