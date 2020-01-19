

namespace Cursed.Models.DataModel.Products
{
    /// <summary>
    /// Model used for products gathering
    /// </summary>
    public class ProductsDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Uid { get; set; }
        public decimal Quantity { get; set; }
        public string QuantityUnit { get; set; }
        public decimal Price { get; set; }
    }
}
