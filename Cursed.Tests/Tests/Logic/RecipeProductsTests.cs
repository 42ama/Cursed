using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Collections.Generic;
using Cursed.Models.Logic;
using Cursed.Models.Entities.Data;
using Cursed.Models.DataModel.RecipeProducts;
using Cursed.Models.DataModel.Shared;
using Cursed.Models.DataModel.Utility;
using Cursed.Models.StaticReferences;

namespace Cursed.Tests.Tests.Logic
{
    [Collection("Tests collection")]
    public class RecipeProductsTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly RecipeProductsLogic logic;

        public RecipeProductsTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new RecipeProductsLogic(fixture.db);
        }

        public async void Dispose()
        {
            await TestsFixture.ClearDatabase(fixture.db);
        }

        private RecipeProductChanges GetRecipeProductChanges()
        {
            return new RecipeProductChanges
            {
                ProductId = 44440,
                RecipeId = 44440,
                Quantity = (decimal)12.3456,
                Type = ProductCatalogTypes.Product
            };
        }

        private IEnumerable<RecipeProductChanges> GetRecipeProductsChanges()
        {
            return new RecipeProductChanges[]
            {
                new RecipeProductChanges
                {
                    RecipeId = 44440,
                    ProductId = 44440,
                    Quantity = 15,
                    Type = ProductCatalogTypes.Product
                },
                new RecipeProductChanges
                {
                    RecipeId = 44440,
                    ProductId = 44441,
                    Quantity = 9,
                    Type = ProductCatalogTypes.Material
                },
                new RecipeProductChanges
                {
                    RecipeId = 44440,
                    ProductId = 44442,
                    Quantity = 10,
                    Type = ProductCatalogTypes.Material
                }
            };
        }

        private IEnumerable<ProductCatalog> GetProductsCatalog()
        {
            return new ProductCatalog[]
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
        }

        [Fact]
        public async void AddRecipeProduct_ToEmptyDbTable_AddedRecipeProductEqualExpectedRecipeProduct()
        {
            // arrange
            var expected = GetRecipeProductChanges();

            // act
            await logic.AddDataModelAsync(expected);

            // assert
            var actual = await fixture.db.RecipeProductChanges.FirstOrDefaultAsync(i => i.ProductId == expected.ProductId && i.RecipeId == expected.RecipeId);
            Assert.Equal(expected.ProductId, actual.ProductId);
            Assert.Equal(expected.RecipeId, actual.RecipeId);
            Assert.Equal(expected.Quantity, actual.Quantity);
            Assert.Equal(expected.Type, actual.Type);
        }

        [Fact]
        public async void RemoveRecipeProductChanges_FromInitializedDbTable_RemovedRecipeProductChangesNotFoundInDb()
        {
            // arrange
            var recipeProductChanges = GetRecipeProductChanges();
            fixture.db.Add(recipeProductChanges);

            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(recipeProductChanges);

            // assert
            var actual = await fixture.db.RecipeProductChanges.FirstOrDefaultAsync(i => i.ProductId == recipeProductChanges.ProductId && i.RecipeId == recipeProductChanges.RecipeId);
            Assert.Null(actual);
        }

        [Fact]
        public async void UpdateRecipeProductChanges_AtInitializedDbTable_UpdatedRecipeProductChangesEqualExpectedRecipeProductChanges()
        {
            // arrange
            var recipeProductChanges = GetRecipeProductChanges();
            fixture.db.Add(recipeProductChanges);
            await fixture.db.SaveChangesAsync();

            var expected = new RecipeProductChanges
            {
                ProductId = recipeProductChanges.ProductId,
                RecipeId = recipeProductChanges.RecipeId,
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
        }

        [Fact]
        public async void GetListRecipeProductsDataModel_FromInitializedDbTables_LogicRecipeProductsDataModelsEqualExpectedRecipeProductsDataModels()
        {
            var recipeProductsChanges = GetRecipeProductsChanges();
            var productsCatalog = GetProductsCatalog();

            fixture.db.ProductCatalog.AddRange(productsCatalog);
            fixture.db.RecipeProductChanges.AddRange(recipeProductsChanges);
            await fixture.db.SaveChangesAsync();
            var expected = new List<RecipeProductsDataModel>
            {
                new RecipeProductsDataModel
                {
                    RecipeId = 44440,
                    ProductId = 44440,
                    Type = ProductCatalogTypes.Product,
                    Quantity = 15,
                    ProductName = "Testesteron",
                    Cas = 4040404,
                    LicenseRequired = false
                },
                new RecipeProductsDataModel
                {
                    RecipeId = 44440,
                    ProductId = 44441,
                    Type = ProductCatalogTypes.Material,
                    Quantity = 9,
                    ProductName = "Testin",
                    Cas = 4040414,
                    LicenseRequired = true
                },
                new RecipeProductsDataModel
                {
                    RecipeId = 44440,
                    ProductId = 44442,
                    Type = ProductCatalogTypes.Material,
                    Quantity = 10,
                    ProductName = "Testonion",
                    Cas = 4041404,
                    LicenseRequired = null
                }
            };

            // act
            var actual = (await logic.GetAllDataModelAsync(expected.First().RecipeId)).ToList();

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
        }
    }
}
