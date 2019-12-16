using System;
using System.Collections.Generic;

namespace Cursed.Models.Entities
{
    public partial class Operation
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int TransactionId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public int StorageFromId { get; set; }
        public int StorageToId { get; set; }

        public virtual ProductCatalog Product { get; set; }
        public virtual Storage StorageFrom { get; set; }
        public virtual Storage StorageTo { get; set; }
        public virtual TransactionBatch Transaction { get; set; }
    }
}
