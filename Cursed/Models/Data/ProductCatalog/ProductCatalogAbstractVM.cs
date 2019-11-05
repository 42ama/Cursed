

namespace Cursed.Models.Data.ProductCatalog
{
    public abstract class ProductCatalogAbstractVM
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string CAS { get; set; }
        public string Type { get; set; }
        public bool LicenseRequired { get; set; }
    }
}
