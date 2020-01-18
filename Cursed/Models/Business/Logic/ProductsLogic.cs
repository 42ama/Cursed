using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Cursed.Models.Context;
using Cursed.Models.DataModel.Products;
using Cursed.Models.Interfaces.LogicCRUD;

namespace Cursed.Models.Logic
{
    /// <summary>
    /// Authetication section logic. Consists of read action for products at storage.
    /// </summary>
    public class ProductsLogic : IReadCollectionByParam<ProductsDataModel>
    {
        private readonly CursedDataContext db;
        public ProductsLogic(CursedDataContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Gather all products at specific storage from database.
        /// </summary>
        /// <param name="key">Id of storage to which products belongs</param>
        /// <returns>All products at specific storage from database. Each product contains more information than Product entity.</returns>
        public async Task<IEnumerable<ProductsDataModel>> GetAllDataModelAsync(object key)
        {
            int storageId = (int)key;
            var query = from p in db.Product
                        where p.StorageId == storageId
                        join pc in db.ProductCatalog on p.Uid equals pc.Id
                        select new ProductsDataModel
                        {
                            Id = p.Id,
                            Name = pc.Name,
                            Price = p.Price,
                            Quantity = p.Quantity,
                            QuantityUnit = p.QuantityUnit,
                            Uid = p.Uid
                        };

            return query;
        }
    }
}
