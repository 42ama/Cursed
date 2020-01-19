using System;
using System.Collections.Generic;

namespace Cursed.Models.Entities.Data
{
    public partial class ProductCatalog
    {
        public ProductCatalog()
        {
            License = new HashSet<License>();
            Product = new HashSet<Product>();
            RecipeProductChanges = new HashSet<RecipeProductChanges>();
            Operation = new HashSet<Operation>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Cas { get; set; }
        public bool? LicenseRequired { get; set; }

        public virtual ICollection<License> License { get; set; }
        public virtual ICollection<Product> Product { get; set; }
        public virtual ICollection<Operation> Operation { get; set; }
        public virtual ICollection<RecipeProductChanges> RecipeProductChanges { get; set; }
    }
}
