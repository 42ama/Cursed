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
using Cursed.Models.Data.Companies;
using Cursed.Models.Entities;
using Cursed.Models.Interfaces.LogicCRUD;
using Cursed.Models.Data.Utility;
using Cursed.Models.Data.Utility.ErrorHandling;
using Cursed.Models.Routing;
using Cursed.Models.Services;

namespace Cursed.Models.LogicValidation
{
    public class StoragesLogicValidation
    {
        private readonly CursedDataContext db;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public StoragesLogicValidation(CursedDataContext db, IErrorHandlerFactory errorHandlerFactory)
        {
            this.db = db;
            this.errorHandlerFactory = errorHandlerFactory;
        }

        public async Task<IErrorHandler> CheckGetSingleDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        public async Task<IErrorHandler> CheckGetSingleUpdateModelAsync(object key)
        {
            return await CheckExists(key);
        }

        public async Task<IErrorHandler> CheckUpdateDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        public async Task<IErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            var statusMessage = await CheckExists(key);

            if (!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            // check related entities
            var products = db.Product.Where(i => i.StorageId == (int)key);
            var operations = db.Operation.Where(i => i.StorageToId == (int)key || i.StorageFromId == (int)key);

            if (products.Any())
            {
                foreach (var product in products)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = "Product.",
                        EntityKey = product.Id,
                        Message = "You must remove dependent Product in Storage first.",
                        RedirectRoute = ProductsRouting.SingleItem
                    });
                }
            }
            if (operations.Any())
            {
                foreach (var operation in operations)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = "Operation.",
                        EntityKey = operation.Id,
                        Message = "You must remove dependent Operation first.",
                        RedirectRoute = OperationsRouting.SingleItem
                    });
                }
            }

            return statusMessage;
        }
        private async Task<IErrorHandler> CheckExists(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Storage.",
                EntityKey = (int)key,
                RedirectRoute = StoragesRouting.SingleItem
            });

            if (await db.Storage.FirstOrDefaultAsync(i => i.Id == (int)key) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Storage.",
                    EntityKey = (int)key,
                    Message = "Storage with this Id is not found.",
                    RedirectRoute = StoragesRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            return statusMessage;
        }
    }
}
