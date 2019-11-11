

namespace Cursed.Models.Data.ProductCatalog
{
    public abstract class ProductCatalogAbstractModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int? CAS { get; set; }
        public string Type { get; set; }
        public bool LicenseRequired { get; set; }
    }
}
