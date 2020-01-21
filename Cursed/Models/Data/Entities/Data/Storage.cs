using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cursed.Models.Entities.Data
{
    public partial class Storage
    {
        public Storage()
        {
            OperationStorageFrom = new HashSet<Operation>();
            OperationStorageTo = new HashSet<Operation>();
            Product = new HashSet<Product>();
        }

        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Range(-90, 90, ErrorMessage = "Latitude must be in range between -90 and 90")]
        public decimal? Latitude { get; set; }
        [Range(-90, 90, ErrorMessage = "Longitude must be in range between -90 and 90")]
        public decimal? Longitude { get; set; }
        [Required]
        public int CompanyId { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<Operation> OperationStorageFrom { get; set; }
        public virtual ICollection<Operation> OperationStorageTo { get; set; }
        public virtual ICollection<Product> Product { get; set; }
    }
}
