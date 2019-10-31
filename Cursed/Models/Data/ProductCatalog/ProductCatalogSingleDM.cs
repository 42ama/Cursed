using System.Collections.Generic;
using Cursed.Models.Data.Utility;

namespace Cursed.Models.Data.ProductCatalog
{
    public class ProductCatalogSingleDM : ProductCatalogAbstractDM
    {
        public List<NameIdContainer> Recipes { get; set; }
        public List<NameIdContainer> Storages { get; set; }
    }
}
