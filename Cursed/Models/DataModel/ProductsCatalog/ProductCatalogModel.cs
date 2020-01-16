using System.Collections.Generic;
using Cursed.Models.DataModel.Utility;
using Cursed.Models.Entities.Data;

namespace Cursed.Models.DataModel.ProductsCatalog
{
    public class ProductCatalogModel : ProductsCatalogAbstractModel
    {
        public List<(License license, bool isValid)> Licenses { get; set; }
        public List<TitleIdContainer> Recipes { get; set; }
        public List<TitleIdContainer> Storages { get; set; }
    }
}
