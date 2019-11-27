using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities;

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
            var entityModel = new RecipeProductChanges
            {
                ProductId = 44440,
                RecipeId = 44440,
                Quantity = (decimal)12.3456,
                Type = Models.Data.Shared.ProductCatalogTypes.Product
            };

            // act
            await logic.AddDataModelAsync(entityModel);

            // assert
            var testModel = await fixture.db.RecipeProductChanges.FirstOrDefaultAsync(i => i.ProductId == entityModel.ProductId && i.RecipeId == entityModel.RecipeId);
            Assert.Equal(entityModel, testModel);

            // dispose
            fixture.db.RecipeProductChanges.Remove(testModel);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void RemoveDataModelAsync()
        {
            // arrange
            var entityModel = new RecipeProductChanges
            {
                ProductId = 44440,
                RecipeId = 44440,
                Quantity = (decimal)12.3456,
                Type = Models.Data.Shared.ProductCatalogTypes.Product
            };
            fixture.db.Add(entityModel);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(entityModel);

            // assert
            var testModel = await fixture.db.RecipeProductChanges.FirstOrDefaultAsync(i => i.ProductId == entityModel.ProductId && i.RecipeId == entityModel.RecipeId);
            Assert.Null(testModel);
        }

        [Fact]
        public async void UpdateDataModelAsync()
        {
            // arrange
            var entityModel = new RecipeProductChanges
            {
                ProductId = 44440,
                RecipeId = 44440,
                Quantity = (decimal)12.3456,
                Type = Models.Data.Shared.ProductCatalogTypes.Product
            };
            fixture.db.Add(entityModel);
            await fixture.db.SaveChangesAsync();

            var updatedModel = new RecipeProductChanges
            {
                ProductId = 44440,
                RecipeId = 44440,
                Quantity = (decimal)78.9012,
                Type = Models.Data.Shared.ProductCatalogTypes.Product
            };

            // act
            await logic.UpdateDataModelAsync(updatedModel);

            // assert
            var testModel = await fixture.db.RecipeProductChanges.FirstOrDefaultAsync(i => i.ProductId == entityModel.ProductId && i.RecipeId == entityModel.RecipeId);
            Assert.Equal(updatedModel.Quantity, testModel.Quantity);

            // dispose
            fixture.db.RecipeProductChanges.Remove(testModel);
            await fixture.db.SaveChangesAsync();
        }
    }
}
