using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities.Data;
using Cursed.Models.StaticReferences;
using Cursed.Models.DataModel.Products;

namespace Cursed.Tests.Tests.Logic
{
    [Collection("Tests collection")]
    public class ProductsTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly ProductsLogic logic;

        public ProductsTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new ProductsLogic(fixture.db);
        }

        public async void Dispose()
        {
            await TestsFixture.ClearDatabase(fixture.db);
        }

        private ProductCatalog GetProductCatalog()
        {
            return new ProductCatalog
            {
                Id = 44440,
                Cas = 4040404,
                Name = "Testesteron",
                LicenseRequired = true,
                Type = ProductCatalogTypes.Product
            };
        }

        private Product[] GetProducts()
        {
            return new Product[]
            {
                new Product {
                    Id = 44440,
                    Uid = 44440,
                    Price = 22.1M,
                    Quantity = 5,
                    QuantityUnit = "mg.",
                    StorageId = 44440
                },
                new Product {
                    Id = 44441,
                    Uid = 44440,
                    Price = 14.1M,
                    Quantity = 6,
                    QuantityUnit = "mg.",
                    StorageId = 44440
                },
            };
        }


        [Fact]
        public async void RemoveRecipe_FromInitializedDbTable_RemovedRecipeNotFoundInDb()
        {
            // arrange
            var productCatalog = GetProductCatalog();
            var products = GetProducts();
            fixture.db.Add(productCatalog);
            fixture.db.Product.AddRange(products);
            await fixture.db.SaveChangesAsync();

            var expected = new ProductsDataModel[]
            {
                new ProductsDataModel
                {
                    Id = 44440,
                    Uid = 44440,
                    Price = 22.1M,
                    Quantity = 5,
                    QuantityUnit = "mg.",
                    Name = productCatalog.Name,
                    Type = productCatalog.Type
                },
                new ProductsDataModel
                {
                    Id = 44441,
                    Uid = 44440,
                    Price = 14.1M,
                    Quantity = 6,
                    QuantityUnit = "mg.",
                    Name = productCatalog.Name,
                    Type = productCatalog.Type
                },
            };
            int storageId = 44440;

            // act
            var actual = await logic.GetAllDataModelAsync(storageId);

            // assert
            foreach (var expectedItem in expected)
            {
                Assert.Contains(actual, actualItem =>
                    expectedItem.Id == actualItem.Id &&
                    expectedItem.Name == actualItem.Name &&
                    expectedItem.Type == actualItem.Type &&
                    expectedItem.Uid == actualItem.Uid &&
                    expectedItem.Price == actualItem.Price &&
                    expectedItem.Quantity == actualItem.Quantity &&
                    expectedItem.QuantityUnit == actualItem.QuantityUnit);
            }
        }

    }
}
