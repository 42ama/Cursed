﻿using System;
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

namespace Cursed.Models.LogicValidation
{
    public class StoragesLogicValidation
    {
        private readonly CursedContext db;
        private readonly AbstractErrorHandlerFactory errorHandlerFactory;

        public StoragesLogicValidation(CursedContext db)
        {
            this.db = db;
            errorHandlerFactory = new StatusMessageFactory();
        }

        public async Task<AbstractErrorHandler> CheckGetSingleDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        public async Task<AbstractErrorHandler> CheckGetSingleUpdateModelAsync(object key)
        {
            return await CheckExists(key);
        }

        public async Task<AbstractErrorHandler> CheckUpdateDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        public async Task<AbstractErrorHandler> CheckRemoveDataModelAsync(object key)
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
                        Message = "Storage have related product in storage."
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
                        Message = "Storage have related operation."
                    });
                }
            }

            return statusMessage;
        }
        private async Task<AbstractErrorHandler> CheckExists(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Storage.", key);

            if (await db.Storage.FirstOrDefaultAsync(i => i.Id == (int)key) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Storage.",
                    EntityKey = key,
                    Message = "No storage with such key found."
                });
            }

            return statusMessage;
        }
    }
}