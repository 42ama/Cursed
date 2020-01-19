using System;
using System.Collections.Generic;
using Cursed.Models.Entities.Data;

namespace Cursed.Models.DataModel.ProductsCatalog
{
    /// <summary>
    /// Model used for single product from catalog gathering
    /// </summary>
    public class ProductCatalogModel : ProductsCatalogAbstractModel
    {
        public List<(License license, bool isValid)> Licenses { get; set; }
        public List<ValueTuple<string, int>> Recipes { get; set; }
        public List<ValueTuple<string, int>> Storages { get; set; }
    }
}
