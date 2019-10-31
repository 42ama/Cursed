

namespace Cursed.Models.Data.ProductCatalog
{
    public class ProductCatalogAllVM : ProductCatalogAbstractVM
    {
        public int LicenseId { get; set; }
        public string GovermentNum { get; set; }
        public string LicensedUntil { get; set; }
        public string LicenseSummary { get; set; }
        public string AttentionColor { get; set; }
        public int RecipesCount { get; set; }
        public int StoragesCount { get; set; }
    }
}
