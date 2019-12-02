using Cursed.Models.Data.Shared;

namespace Cursed.Models.Data.ProductsCatalog
{
    public class ProductsCatalogModel : ProductsCatalogAbstractModel
    {
        public bool IsValid { get; set; }
        public int RecipesCount { get; set; }
        public int StoragesCount { get; set; }
    }
}
