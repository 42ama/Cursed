using Cursed.Models.Data.Shared;

namespace Cursed.Models.Data.ProductCatalog
{
    public class ProductCatalogAllVM : ProductCatalogAbstractVM
    {
        public int LicenseId { get; set; }
        public int GovermentNum { get; set; }
        public string Date { get; set; }
        public bool IsValid { get; set; }
        public int RecipesCount { get; set; }
        public int StoragesCount { get; set; }
    }
}
