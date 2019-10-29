using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Data
{
    public class ProductCatalogDataModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int? CAS { get; set; }
        public string Type { get; set; }
        public bool? LicenseRequired { get; set; }
        public int? GovermentNum { get; set; }
        public DateTime? LicensedUntil { get; set; }
    }
}
