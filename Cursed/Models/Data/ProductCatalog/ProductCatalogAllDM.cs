using Cursed.Models.Data.Shared;

namespace Cursed.Models.Data.ProductCatalog
{
    public class ProductCatalogAllDM : ProductCatalogAbstractDM
    {
        public LicenseValid License { get; set; }
        public int RecipesCount { get; set; }
        public int StoragesCount { get; set; }
    }
}
