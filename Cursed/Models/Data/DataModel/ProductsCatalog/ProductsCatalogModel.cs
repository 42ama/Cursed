

namespace Cursed.Models.DataModel.ProductsCatalog
{
    /// <summary>
    /// Model used for multiple products from catalog gathering
    /// </summary>
    public class ProductsCatalogModel : ProductsCatalogAbstractModel
    {
        public bool IsValid { get; set; }
        public int RecipesCount { get; set; }
        public int StoragesCount { get; set; }
    }
}
