using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.LogicValidation;
using Cursed.Models.Services;
using Cursed.Models.Entities.Data;
using Cursed.Models.DataModel.Shared;
using Cursed.Tests.Stubs;
using Cursed.Models.DataModel.Utility.ErrorHandling;

namespace Cursed.Tests.IntegrationTests
{
    [Collection("Tests collection")]
    public class TransactionCloseTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly TransactionsLogicValidation logicValidation;
        private readonly OperationDataValidation operationValidation;

        public TransactionCloseTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            operationValidation = new OperationDataValidation(fixture.db);
            logicValidation = new TransactionsLogicValidation(fixture.db, operationValidation, new StatusMessageFactory());
        }

        public async void Dispose()
        {
            await TestsFixture.ClearDatabase(fixture.db);
        }

        private IEnumerable<Product> GetProducts()
        {
            return new Product[]
            {
                new Product
                {
                    Id = 44440,
                    Uid = 44440,
                    StorageId = 44440,
                    Price = 15,
                    Quantity = 10,
                    QuantityUnit = "mg."
                },
                new Product
                {
                    Id = 44441,
                    Uid = 44440,
                    StorageId = 44441,
                    Price = 17,
                    Quantity = 9999,
                    QuantityUnit = "mg."
                }
            };
        }

        private ProductCatalog GetProductCatalog()
        {
            return new ProductCatalog
            {
                Id = 44440,
                Cas = 4040404,
                LicenseRequired = true,
                Name = "Testotin",
                Type = ProductCatalogTypes.Product
            };
        }

        private TransactionBatch GetTransaction()
        {
            return new TransactionBatch
            {
                Id = 44440,
                Date = DateTime.UtcNow,
                CompanyId = 44440,
                Type = TransactionTypes.Income,
                IsOpen = true,
                Comment = "Why chicken cross the road?"
            };
        }

        private IEnumerable<Storage> GetStorages()
        {
            return new Storage[]
            {
                new Storage
                {
                    Id = 44440,
                    Name = "Test storage #1",
                    Latitude = (decimal)12.3456,
                    Longitude = (decimal)78.9012,
                    CompanyId = 44440
                },
                new Storage
                {
                    Id = 44441,
                    Name = "Test storage #2",
                    Latitude = (decimal)12.3456,
                    Longitude = (decimal)78.9012,
                    CompanyId = 44440
                }
            };
        }

        private Operation GetOperationIncome()
        {
            return new Operation
            {
                TransactionId = 44440,
                Id = 44440,
                ProductId = 44440,
                StorageToId = 44440,
                StorageFromId = 44441,
                Price = 15,
                Quantity = 22
            };
        }

        private Operation GetOperationOutcome()
        {
            return new Operation
            {
                TransactionId = 44440,
                Id = 44441,
                ProductId = 44440,
                StorageToId = 44441,
                StorageFromId = 44440,
                Price = 15,
                Quantity = 20
            };
        }

        [Fact]
        public async void CheckCloseTransaction_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var productsCatalog = GetProductCatalog();
            var products = GetProducts();
            var storages = GetStorages();
            var transactions = GetTransaction();
            var operations = GetOperationIncome();
            fixture.db.AddRange(productsCatalog);
            fixture.db.AddRange(products);
            fixture.db.AddRange(storages);
            fixture.db.AddRange(transactions);
            fixture.db.AddRange(operations);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckCloseTransactionAsync(transactions.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckCloseTransaction_FromInitializedDbTable_ErrorHandlerIsCompletedFalseHaveProblemWithProductIdAsEntityKey()
        {
            // arrange
            var productsCatalog = GetProductCatalog();
            var products = GetProducts();
            var storages = GetStorages();
            var transactions = GetTransaction();
            var operations = GetOperationOutcome();
            fixture.db.AddRange(productsCatalog);
            fixture.db.AddRange(products);
            fixture.db.AddRange(storages);
            fixture.db.AddRange(transactions);
            fixture.db.AddRange(operations);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckCloseTransactionAsync(transactions.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
            Assert.Contains(statusMessage.Problems, problem =>
            problem.EntityKey == "44440");
        }
    }
}
