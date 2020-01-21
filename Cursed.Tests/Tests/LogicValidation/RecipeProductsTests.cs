using System;
using Xunit;
using Cursed.Models.LogicValidation;
using Cursed.Models.Entities.Data;
using Cursed.Models.StaticReferences;
using Cursed.Models.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Cursed.Models.DataModel.ErrorHandling;

namespace Cursed.Tests.Tests.LogicValidation
{
    [Collection("Tests collection")]
    public class RecipeProductsTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly RecipeProductsLogicValidation logicValidation;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public RecipeProductsTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            errorHandlerFactory = new StatusMessageFactory();
            logicValidation = new RecipeProductsLogicValidation(fixture.db, errorHandlerFactory);
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
                Name = "Testotin"
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
        public async void CheckRemoveRecipeProduct_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var recipeProductChanges = GetRecipeProductChanges();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync((recipeProductChanges.RecipeId, recipeProductChanges.ProductId));

            // assert
            Assert.False(statusMessage.IsCompleted);

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

        [Fact]
        public void CheckValidateModel_WithoutErrors_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Technological procces."
            });

            // act
            var actual = logicValidation.ValidateModel(statusMessage, new ModelStateDictionary());

            // assert
            Assert.True(actual.IsCompleted);
        }

        [Fact]
        public void CheckValidateModel_WithErrors_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Technological procces."
            });
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("key", "error");

            // act
            var actual = logicValidation.ValidateModel(statusMessage, modelState);

            // assert
            Assert.False(actual.IsCompleted);
        }
    }
}
