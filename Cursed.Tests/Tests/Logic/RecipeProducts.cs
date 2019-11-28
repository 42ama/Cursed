using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Collections.Generic;
using Cursed.Models.Logic;
using Cursed.Models.Entities;
using Cursed.Models.Data.RecipeProducts;
using Cursed.Models.Data.Shared;
using Cursed.Models.Data.Utility;

namespace Cursed.Tests.Tests.Logic
{
    public class RecipeProducts : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture fixture;
        private readonly RecipeProductsLogic logic;

        public RecipeProducts(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new RecipeProductsLogic(fixture.db);
        }

        [Fact]
        public async void AddDataModelAsync()
        {
            // arrange
            var expected = new RecipeProductChanges
            {
                ProductId = 44440,
                RecipeId = 44440,
                Quantity = (decimal)12.3456,
                Type = ProductCatalogTypes.Product
            };

            // act
            await logic.AddDataModelAsync(expected);

            // assert
            var actual = await fixture.db.RecipeProductChanges.FirstOrDefaultAsync(i => i.ProductId == expected.ProductId && i.RecipeId == expected.RecipeId);
            Assert.Equal(expected.ProductId, actual.ProductId);
            Assert.Equal(expected.RecipeId, actual.RecipeId);
            Assert.Equal(expected.Quantity, actual.Quantity);
            Assert.Equal(expected.Type, actual.Type);

            // dispose
            fixture.db.RecipeProductChanges.Remove(actual);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void RemoveDataModelAsync()
        {
            // arrange
            var recipeProductChanges = new RecipeProductChanges
            {
                ProductId = 44440,
                RecipeId = 44440,
                Quantity = (decimal)12.3456,
                Type = ProductCatalogTypes.Product
            };
            fixture.db.Add(recipeProductChanges);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(recipeProductChanges);

            // assert
            var actual = await fixture.db.RecipeProductChanges.FirstOrDefaultAsync(i => i.ProductId == recipeProductChanges.ProductId && i.RecipeId == recipeProductChanges.RecipeId);
            Assert.Null(actual);
        }

        [Fact]
        public async void UpdateDataModelAsync()
        {
            // arrange
            var recipeProductChanges = new RecipeProductChanges
            {
                ProductId = 44440,
                RecipeId = 44440,
                Quantity = (decimal)12.3456,
                Type = ProductCatalogTypes.Product
            };
            fixture.db.Add(recipeProductChanges);
            await fixture.db.SaveChangesAsync();

            var expected = new RecipeProductChanges
            {
                ProductId = 44440,
                RecipeId = 44440,
                Quantity = (decimal)78.9012,
                Type = ProductCatalogTypes.Product
            };

            // act
            await logic.UpdateDataModelAsync(expected);

            // assert
            var actual = await fixture.db.RecipeProductChanges.FirstOrDefaultAsync(i => i.ProductId == recipeProductChanges.ProductId && i.RecipeId == recipeProductChanges.RecipeId);
            Assert.Equal(expected.ProductId, actual.ProductId);
            Assert.Equal(expected.RecipeId, actual.RecipeId);
            Assert.Equal(expected.Quantity, actual.Quantity);
            Assert.Equal(expected.Type, actual.Type);

            // dispose
            fixture.db.RecipeProductChanges.Remove(actual);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void GetAllDataModelAsync()
        {
            int recipeId = 44440;
            var recipeProductsChanges = new RecipeProductChanges[]
            {
                new RecipeProductChanges
                {
                    RecipeId = recipeId,
                    ProductId = 44440,
                    Quantity = 15,
                    Type = ProductCatalogTypes.Product
                },
                new RecipeProductChanges
                {
                    RecipeId = recipeId,
                    ProductId = 44441,
                    Quantity = 9,
                    Type = ProductCatalogTypes.Material
                },
                new RecipeProductChanges
                {
                    RecipeId = recipeId,
                    ProductId = 44442,
                    Quantity = 10,
                    Type = ProductCatalogTypes.Material
                }
            };
            var productsCatalog = new ProductCatalog[]
            {
                new ProductCatalog
                {
                    Id = 44440,
                    Name = "Testesteron",
                    Cas = 4040404,
                    LicenseRequired = false
                },
                new ProductCatalog
                {
                    Id = 44441,
                    Name = "Testin",
                    Cas = 4040414,
                    LicenseRequired = true
                },
                new ProductCatalog
                {
                    Id = 44442,
                    Name = "Testonion",
                    Cas = 4041404
                }
            };

            fixture.db.ProductCatalog.AddRange(productsCatalog);
            fixture.db.RecipeProductChanges.AddRange(recipeProductsChanges);
            await fixture.db.SaveChangesAsync();
            var expected = new List<RecipeProductsDataModel>
            {
                new RecipeProductsDataModel
                {
                    RecipeId = recipeId,
                    ProductId = 44440,
                    Type = ProductCatalogTypes.Product,
                    Quantity = 15,
                    ProductName = "Testesteron",
                    Cas = 4040404,
                    LicenseRequired = false
                },
                new RecipeProductsDataModel
                {
                    RecipeId = recipeId,
                    ProductId = 44441,
                    Type = ProductCatalogTypes.Material,
                    Quantity = 9,
                    ProductName = "Testin",
                    Cas = 4040414,
                    LicenseRequired = true
                },
                new RecipeProductsDataModel
                {
                    RecipeId = recipeId,
                    ProductId = 44442,
                    Type = ProductCatalogTypes.Material,
                    Quantity = 10,
                    ProductName = "Testonion",
                    Cas = 4041404,
                    LicenseRequired = null
                }
            };

            // act
            var actual = (await logic.GetAllDataModelAsync(recipeId)).ToList();

            // assert
            foreach (var expectedItem in expected)
            {
                Assert.Contains(actual, actualItem =>
                    expectedItem.RecipeId == actualItem.RecipeId &&
                    expectedItem.ProductId == actualItem.ProductId &&
                    expectedItem.Type == actualItem.Type &&
                    expectedItem.Quantity == actualItem.Quantity &&
                    expectedItem.ProductName == actualItem.ProductName &&
                    expectedItem.Cas == actualItem.Cas &&
                    expectedItem.LicenseRequired == actualItem.LicenseRequired);
            }

            // dispose
            fixture.db.RecipeProductChanges.RemoveRange(recipeProductsChanges);
            fixture.db.ProductCatalog.RemoveRange(productsCatalog);
            await fixture.db.SaveChangesAsync();
        }
    }
}
