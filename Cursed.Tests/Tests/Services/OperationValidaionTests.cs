using System;
using Xunit;
using Cursed.Models.Entities;
using Cursed.Models.Services;
using System.Collections.Generic;
using Cursed.Models.Data.Shared;
using System.Text;

namespace Cursed.Tests.Tests.Services
{
    [Collection("Tests collection")]
    public class OperationValidaionTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly OperationValidation validation;

        public OperationValidaionTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            validation = new OperationValidation(fixture.db);
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
                TransactionId = 44441,
                Id = 44441,
                ProductId = 44440,
                StorageToId = 44441,
                StorageFromId = 44440,
                Price = 15,
                Quantity = 20
            };
        }

        [Fact]
        public async void IsOperationValid_DbInitializedOperationValid_ReturnsTrue()
        {
            // arrange
            var productsCatalog = GetProductCatalog();
            var products = GetProducts();
            var storages = GetStorages();
            fixture.db.AddRange(productsCatalog);
            fixture.db.AddRange(products);
            fixture.db.AddRange(storages);
            await fixture.db.SaveChangesAsync();

            var expected = GetOperationIncome();

            // act
            var actual = await validation.IsValidAsync(expected);

            // assert
            Assert.True(actual);
        }

        [Fact]
        public async void IsOperationValid_DbInitializedOperationHaveWrongQuantity_ReturnsFalse()
        {
            // arrange
            var productsCatalog = GetProductCatalog();
            var products = GetProducts();
            var storages = GetStorages();
            fixture.db.AddRange(productsCatalog);
            fixture.db.AddRange(products);
            fixture.db.AddRange(storages);
            await fixture.db.SaveChangesAsync();

            var expected = GetOperationOutcome();

            // act
            var actual = await validation.IsValidAsync(expected);

            // assert
            Assert.False(actual);
        }
    }
}
