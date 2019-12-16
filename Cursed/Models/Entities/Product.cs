using System;
using System.Collections.Generic;

namespace Cursed.Models.Entities
{
    public partial class Product
    {
        public int Id { get; set; }
        public int Uid { get; set; }
        public decimal Quantity { get; set; }
        public string QuantityUnit { get; set; }
        public decimal Price { get; set; }
        public int StorageId { get; set; }

        public virtual Storage Storage { get; set; }
        public virtual ProductCatalog U { get; set; }
    }
}
