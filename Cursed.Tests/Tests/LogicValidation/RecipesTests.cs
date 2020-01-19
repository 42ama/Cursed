using System;
using Xunit;
using Cursed.Models.LogicValidation;
using Cursed.Models.Entities.Data;
using Cursed.Models.StaticReferences;
using Cursed.Models.Services;

namespace Cursed.Tests.Tests.LogicValidation
{
    [Collection("Tests collection")]
    public class RecipesTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly RecipesLogicValidation logicValidation;

        public RecipesTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logicValidation = new RecipesLogicValidation(fixture.db, new StatusMessageFactory());
        }

        public async void Dispose()
        {
            await TestsFixture.ClearDatabase(fixture.db);
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

        private RecipeInheritance GetRecipeInheritance()
        {
            return new RecipeInheritance
            {
                ParentId = 44440,
                ChildId = 44441
            };
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

        private TechProcess GetTechProcess()
        {
            return new TechProcess
            {
                FacilityId = 44440,
                RecipeId = 44440,
                DayEfficiency = (decimal)12.3456
            };
        }

        [Fact]
        public async void CheckRemoveRecipe_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var recipe = GetRecipe();
            fixture.db.Add(recipe);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(recipe.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckRemoveRecipe_FromInitializedDbTableWithRelatedEntities_ErrorHandlerIsCompletedFalseHaveSpecificProblems()
        {
            // arrange
            var recipe = GetRecipe();
            var recipeInheritance = GetRecipeInheritance();
            var techProcess = GetTechProcess();
            var productChanges = GetRecipeProductChanges();
            fixture.db.Add(recipe);
            fixture.db.Add(recipeInheritance);
            fixture.db.Add(techProcess);
            fixture.db.Add(productChanges);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(recipe.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
            Assert.Contains(statusMessage.Problems, problem =>
                problem.Entity == "Recipe." && Int32.Parse(problem.EntityKey) == recipeInheritance.ChildId);
            Assert.Contains(statusMessage.Problems, problem =>
                problem.Entity.Contains("Technological process.") &&
                Int32.Parse(problem.EntityKey) == techProcess.FacilityId);
            Assert.Contains(statusMessage.Problems, problem =>
                problem.Entity.Contains("Product changes.") &&
                Int32.Parse(problem.EntityKey) == productChanges.RecipeId);
        }

        [Fact]
        public async void CheckGetRecipe_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var recipe = GetRecipe();

            // act
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(recipe.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetRecipe_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var recipe = GetRecipe();
            fixture.db.Add(recipe);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(recipe.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetRecipeForUpdate_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var recipe = GetRecipe();

            // act
            var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(recipe.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetRecipeForUpdate_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var recipe = GetRecipe();
            fixture.db.Add(recipe);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(recipe.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateRecipe_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var recipe = GetRecipe();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(recipe.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateRecipe_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var recipe = GetRecipe();
            fixture.db.Add(recipe);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(recipe.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

    }
}
