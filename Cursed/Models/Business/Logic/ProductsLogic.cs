using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Cursed.Models.Context;
using Cursed.Models.DataModel.Products;
using Cursed.Models.Interfaces.LogicCRUD;

namespace Cursed.Models.Logic
{
    public class ProductsLogic : IReadCollectionByParam<ProductsDataModel>
    {
        private readonly CursedDataContext db;
        public ProductsLogic(CursedDataContext db)
        {
            this.db = db;
        }

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
                            Type = pc.Type,
                            Price = p.Price,
                            Quantity = p.Quantity,
                            QuantityUnit = p.QuantityUnit,
                            Uid = p.Uid
                        };

            return query;
        }
    }
}
