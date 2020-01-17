using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.DataModel.Products
{
    public class ProductContainer
    {
        public int Id { get; set; }
        public int Uid { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public string QuantityUnit { get; set; }
        public decimal Price { get; set; }
    }
}
