using Cursed.Models.Data.Shared;

namespace Cursed.Models.Data.ProductsCatalog
{
    public class ProductsCatalogModel : ProductsCatalogAbstractModel
    {
        public LicenseValid License { get; set; }
        public int RecipesCount { get; set; }
        public int StoragesCount { get; set; }
    }
}
