using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.DataModel.RecipeProducts
{
    public class RecipeProductsDataModel
    {
        public int RecipeId { get; set; }
        public int ProductId { get; set; }
        public string Type { get; set; }
        public decimal Quantity { get; set; }
        public string ProductName { get; set; }
        public int? Cas { get; set; }
        public bool? LicenseRequired { get; set; }
    }
}
