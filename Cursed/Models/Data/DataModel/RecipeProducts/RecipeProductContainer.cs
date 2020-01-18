

namespace Cursed.Models.DataModel.RecipeProducts
{
    /// <summary>
    /// Support model used for products to recipe relation gathering
    /// </summary>
    public class RecipeProductContainer
    {
        public int ProductId { get; set; }
        public int? CAS { get; set; }
        public string ProductName { get; set; }
        public string Type { get; set; }
        public decimal Quantity { get; set; } 
    }
}
