using Cursed.Models.DataModel.Products;
using System.Collections.Generic;

namespace Cursed.Models.DataModel.Storages
{
    /// <summary>
    /// Model used for single storage data gathering
    /// </summary>
    public class StorageModel : StoragesAbstractModel
    {
        public List<ProductContainer> Products { get; set; }
    }
}
