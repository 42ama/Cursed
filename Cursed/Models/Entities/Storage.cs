using System;
using System.Collections.Generic;

namespace Cursed.Models.Entities
{
    public partial class Storage
    {
        public Storage()
        {
            OperationStorageFrom = new HashSet<Operation>();
            OperationStorageTo = new HashSet<Operation>();
            Product = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<Operation> OperationStorageFrom { get; set; }
        public virtual ICollection<Operation> OperationStorageTo { get; set; }
        public virtual ICollection<Product> Product { get; set; }
    }
}
