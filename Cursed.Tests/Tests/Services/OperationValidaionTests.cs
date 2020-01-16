using System;
using Xunit;
using Cursed.Models.Entities.Data;
using Cursed.Models.Services;
using System.Collections.Generic;
using Cursed.Models.DataModel.Shared;
using System.Text;
using Cursed.Models.DataModel.Utility.ErrorHandling;

namespace Cursed.Tests.Tests.Services
{
    [Collection("Tests collection")]
    public class OperationValidaionTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly OperationDataValidation validation;

        public OperationValidaionTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            validation = new OperationDataValidation(fixture.db);
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
        public async void IsOperationValid_DbInitializedOperationValid_StatusMessageReturnsTrue()
        {
            // arrange
            var productsCatalog = GetProductCatalog();
            var products = GetProducts();
            var storages = GetStorages();
            var transactions = GetTransaction();
            fixture.db.AddRange(productsCatalog);
            fixture.db.AddRange(products);
            fixture.db.AddRange(storages);
            fixture.db.AddRange(transactions);
            await fixture.db.SaveChangesAsync();

            var expected = GetOperationIncome();

            // act
            var actual = await validation.IsValidAsync(expected);

            // assert
            Assert.True(actual.IsCompleted);
        }

        [Fact]
        public async void IsOperationValid_DbInitializedOperationHaveWrongQuantity_StatusMessageFalseExpectedProblemContained()
        {
            // arrange
            var productsCatalog = GetProductCatalog();
            var products = GetProducts();
            var storages = GetStorages();
            var transactions = GetTransaction();
            fixture.db.AddRange(productsCatalog);
            fixture.db.AddRange(products);
            fixture.db.AddRange(storages);
            fixture.db.AddRange(transactions);
            await fixture.db.SaveChangesAsync();

            var expected = GetOperationOutcome();
            var expectedProblem = new Problem
            {
                Entity = "Product at storage from.",
                EntityKey = expected.ProductId.ToString(),
                Message = $"Quantity of product at storage from (10) is lesser, then " +
                        $"operation is trying to withdraw ({expected.Quantity})."
            };

            // act 
            var actual = await validation.IsValidAsync(expected);

            // assert
            Assert.False(actual.IsCompleted);
            Assert.Contains(actual.Problems, actualProblem =>
                actualProblem.Message == expectedProblem.Message &&
                actualProblem.Entity == expectedProblem.Entity &&
                actualProblem.EntityKey == expectedProblem.EntityKey);
        }

        [Fact]
        public async void IsOperationValid_DbInitializedOperationHaveWrongProductId_StatusMessageFalseExpectedProblemContained()
        {
            // arrange
            var productsCatalog = GetProductCatalog();
            var products = GetProducts();
            var storages = GetStorages();
            var transactions = GetTransaction();
            fixture.db.AddRange(productsCatalog);
            fixture.db.AddRange(products);
            fixture.db.AddRange(storages);
            fixture.db.AddRange(transactions);
            await fixture.db.SaveChangesAsync();

            var expected = GetOperationOutcome();
            expected.ProductId = 0;
            var expectedProblem = new Problem
            {
                Entity = $"Product at storage from.",
                EntityKey = expected.ProductId.ToString()
            };

            // act 
            var actual = await validation.IsValidAsync(expected);

            // assert
            Assert.False(actual.IsCompleted);
            Assert.Contains(actual.Problems, actualProblem => 
                actualProblem.Entity == expectedProblem.Entity &&
                actualProblem.EntityKey == expectedProblem.EntityKey);
        }

        [Fact]
        public async void IsOperationValid_DbInitializedOperationHaveWrongTransactionId_StatusMessageFalseExpectedProblemContained()
        {
            // arrange
            var productsCatalog = GetProductCatalog();
            var products = GetProducts();
            var storages = GetStorages();
            var transactions = GetTransaction();
            fixture.db.AddRange(productsCatalog);
            fixture.db.AddRange(products);
            fixture.db.AddRange(storages);
            fixture.db.AddRange(transactions);
            await fixture.db.SaveChangesAsync();

            var expected = GetOperationOutcome();
            expected.TransactionId = 0;
            var expectedProblem = new Problem
            {
                Entity = $"Transaction.",
                EntityKey = expected.TransactionId.ToString()
            };

            // act 
            var actual = await validation.IsValidAsync(expected);

            // assert
            Assert.False(actual.IsCompleted);
            Assert.Contains(actual.Problems, actualProblem =>
                actualProblem.Entity == expectedProblem.Entity &&
                actualProblem.EntityKey == expectedProblem.EntityKey);
        }

        [Fact]
        public async void IsOperationValid_DbInitializedOperationHaveWrongStorageToId_StatusMessageFalseExpectedProblemContained()
        {
            // arrange
            var productsCatalog = GetProductCatalog();
            var products = GetProducts();
            var storages = GetStorages();
            var transactions = GetTransaction();
            fixture.db.AddRange(productsCatalog);
            fixture.db.AddRange(products);
            fixture.db.AddRange(storages);
            fixture.db.AddRange(transactions);
            await fixture.db.SaveChangesAsync();

            var expected = GetOperationOutcome();
            expected.StorageToId = 0;
            var expectedProblem = new Problem
            {
                Entity = $"Storage to product coming.",
                EntityKey = expected.StorageToId.ToString()
            };

            // act 
            var actual = await validation.IsValidAsync(expected);

            // assert
            Assert.False(actual.IsCompleted);
            Assert.Contains(actual.Problems, actualProblem =>
                actualProblem.Entity == expectedProblem.Entity &&
                actualProblem.EntityKey == expectedProblem.EntityKey);
        }

        [Fact]
        public async void IsOperationValid_DbInitializedOperationHaveWrongStorageFromId_StatusMessageFalseExpectedProblemContained()
        {
            // arrange
            var productsCatalog = GetProductCatalog();
            var products = GetProducts();
            var storages = GetStorages();
            var transactions = GetTransaction();
            fixture.db.AddRange(productsCatalog);
            fixture.db.AddRange(products);
            fixture.db.AddRange(storages);
            fixture.db.AddRange(transactions);
            await fixture.db.SaveChangesAsync();

            var expected = GetOperationOutcome();
            expected.StorageFromId = 0;
            var expectedProblem = new Problem
            {
                Entity = "Storage from product coming.",
                EntityKey = expected.StorageFromId.ToString()
            };

            // act 
            var actual = await validation.IsValidAsync(expected);

            // assert
            Assert.False(actual.IsCompleted);
            Assert.Contains(actual.Problems, actualProblem =>
                actualProblem.Entity == expectedProblem.Entity &&
                actualProblem.EntityKey == expectedProblem.EntityKey);
        }
    }
}
