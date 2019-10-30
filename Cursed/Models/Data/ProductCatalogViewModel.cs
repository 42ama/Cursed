using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Data
{
    public class ProductCatalogViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string CAS { get; set; }
        public string Type { get; set; }
        public string GovermentNum { get; set; }
        public string LicensedUntil { get; set; }
        public string LicenseSummary { get; set; }
        public string AttentionColor { get; set; }
        public int RecipesCount { get; set; }
        public int StoragesCount { get; set; }
    }
}
