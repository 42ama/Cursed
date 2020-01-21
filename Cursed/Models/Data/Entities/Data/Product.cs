using System;
using System.ComponentModel.DataAnnotations;

namespace Cursed.Models.Entities.Data
{
    public partial class Product
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int Uid { get; set; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Quantity must be positive")]
        public decimal Quantity { get; set; }
        [Required]
        public string QuantityUnit { get; set; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Price must be positive")]
        public decimal Price { get; set; }
        [Required]
        public int StorageId { get; set; }

        public virtual Storage Storage { get; set; }
        public virtual ProductCatalog U { get; set; }
    }
}
