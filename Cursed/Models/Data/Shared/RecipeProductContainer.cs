using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Data.Shared
{
    public class RecipeProductContainer
    {
        public int ProductId { get; set; }
        public int? CAS { get; set; }
        public string ProductName { get; set; }
        public string Type { get; set; }
        public decimal Quantity { get; set; } 
    }
}
