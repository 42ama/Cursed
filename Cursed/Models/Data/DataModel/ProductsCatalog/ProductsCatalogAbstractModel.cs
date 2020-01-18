

namespace Cursed.Models.DataModel.ProductsCatalog
{
    public abstract class ProductsCatalogAbstractModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int? CAS { get; set; }
        public bool LicenseRequired { get; set; }
    }
}
