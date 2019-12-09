using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Cursed.Models.Context;
using Cursed.Models.Data.Products;
using Cursed.Models.Entities;
using Cursed.Models.Interfaces.LogicCRUD;
using Cursed.Models.Data.Utility;
using Cursed.Models.Data.Utility.ErrorHandling;

namespace Cursed.Models.Logic
{
    public class ProductsLogic : IReadCollectionByParam<ProductsDataModel>
    {
        private readonly CursedContext db;
        private readonly AbstractErrorHandlerFactory errorHandlerFactory;
        public ProductsLogic(CursedContext db)
        {
            this.db = db;
            errorHandlerFactory = new StatusMessageFactory();
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
