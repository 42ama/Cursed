

namespace Cursed.Models.DataModel.RecipeProducts
{
    /// <summary>
    /// Model used for products to recipe relation gathering
    /// </summary>
    public class RecipeProductsDataModel
    {
        public int RecipeId { get; set; }
        public int ProductId { get; set; }
        public string Type { get; set; }
        public decimal Quantity { get; set; }
        public string ProductName { get; set; }
        public int? Cas { get; set; }
        public bool? LicenseRequired { get; set; }
    }
}
