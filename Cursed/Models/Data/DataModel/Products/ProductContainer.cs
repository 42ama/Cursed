﻿

namespace Cursed.Models.DataModel.Products
{
    /// <summary>
    /// Model used for products at storage gathering
    /// </summary>
    public class ProductContainer
    {
        public int Id { get; set; }
        public int Uid { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public string QuantityUnit { get; set; }
        public decimal Price { get; set; }
    }
}
