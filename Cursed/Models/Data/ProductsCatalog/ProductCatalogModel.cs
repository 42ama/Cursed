using System.Collections.Generic;
using Cursed.Models.Data.Utility;
using Cursed.Models.Data.Shared;

namespace Cursed.Models.Data.ProductsCatalog
{
    public class ProductCatalogModel : ProductsCatalogAbstractModel
    {
        public List<LicenseValid> Licenses { get; set; }
        public List<TitleIdContainer> Recipes { get; set; }
        public List<TitleIdContainer> Storages { get; set; }
    }
}
