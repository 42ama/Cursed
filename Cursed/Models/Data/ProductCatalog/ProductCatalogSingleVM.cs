using System.Collections.Generic;
using Cursed.Models.Data.Utility;
using Cursed.Models.Data.Shared;

namespace Cursed.Models.Data.ProductCatalog
{
    public class ProductCatalogSingleVM : ProductCatalogAbstractVM
    {
        public bool LicenseRequired { get; set; }
        public List<LicenseValid> Licenses { get; set; }
        public List<TitleIdContainer> Recipes { get; set; }
        public List<TitleIdContainer> Storages { get; set; }
    }
}
