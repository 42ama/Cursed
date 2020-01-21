using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cursed.Models.Entities.Data
{
    public partial class Operation
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int TransactionId { get; set; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Quantity must be positive")]
        public decimal Quantity { get; set; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Price must be positive")]
        public decimal Price { get; set; }
        [Required]
        public int StorageFromId { get; set; }
        [Required]
        public int StorageToId { get; set; }

        public virtual ProductCatalog Product { get; set; }
        public virtual Storage StorageFrom { get; set; }
        public virtual Storage StorageTo { get; set; }
        public virtual TransactionBatch Transaction { get; set; }
    }
}
