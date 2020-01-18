

namespace Cursed.Models.DataModel.ProductsCatalog
{
    /// <summary>
    /// Model used as base for product from catalog data gathering 
    /// </summary>
    public abstract class ProductsCatalogAbstractModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int? CAS { get; set; }
        public bool LicenseRequired { get; set; }
    }
}
