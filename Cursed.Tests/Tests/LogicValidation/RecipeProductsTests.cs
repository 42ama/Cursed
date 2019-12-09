using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.LogicValidation;
using Cursed.Models.Entities;
using Cursed.Models.Data.Facilities;
using Cursed.Models.Data.Shared;
using Cursed.Tests.Extensions;
using Cursed.Tests.Stubs;

namespace Cursed.Tests.Tests.LogicValidation
{
    [Collection("Tests collection")]
    public class RecipeProductsTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly RecipeProductsLogicValidation logicValidation;

        public RecipeProductsTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logicValidation = new RecipeProductsLogicValidation(fixture.db);
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

        private Recipe GetRecipe()
        {
            return new Recipe
            {
                Id = 44440,
                Content = "Test content",
                GovermentApproval = true,
                TechApproval = false
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
        
        [Fact]
        public async void CheckRemoveRecipeProduct_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var recipeProductChanges = GetRecipeProductChanges();
            fixture.db.Add(recipeProductChanges);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync((recipeProductChanges.RecipeId, recipeProductChanges.ProductId));

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckRemoveRecipeProduct_FromInitializedDbTableWithRelatedEntities_ErrorHandlerIsCompletedFalseHaveSpecificProblems()
        {
            // arrange
            var recipeProductChanges = GetRecipeProductChanges();
            var recipe = GetRecipe();
            var productCatalog = GetProductCatalog();
            fixture.db.Add(recipeProductChanges);
            fixture.db.Add(recipe);
            fixture.db.Add(productCatalog);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync((recipeProductChanges.RecipeId, recipeProductChanges.ProductId));

            // assert
            Assert.False(statusMessage.IsCompleted);
            Assert.Contains(statusMessage.Problems, problem =>
            problem.Entity == "Product in catalog." && (int)problem.EntityKey == productCatalog.Id);
            Assert.Contains(statusMessage.Problems, problem =>
            problem.Entity == "Recipe." && (int)problem.EntityKey == recipe.Id);

        }

        [Fact]
        public async void CheckUpdateRecipeProduct_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var recipeProductChanges = GetRecipeProductChanges();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync((recipeProductChanges.RecipeId, recipeProductChanges.ProductId));

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateRecipeProduct_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var recipeProductChanges = GetRecipeProductChanges();
            fixture.db.Add(recipeProductChanges);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync((recipeProductChanges.RecipeId, recipeProductChanges.ProductId));

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

    }
}
