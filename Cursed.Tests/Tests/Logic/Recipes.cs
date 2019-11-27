using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities;

namespace Cursed.Tests.Tests.Logic
{
    public class Recipes : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture fixture;
        private readonly RecipesLogic logic;

        public Recipes(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new RecipesLogic(fixture.db);
        }

        [Fact]
        public async void AddDataModelAsync()
        {
            // arrange
            var entityModel = new Recipe
            {
                Id = 44440,
                Content = "Test content",
                GovermentApproval = true,
                TechApproval = false
            };

            // act
            await logic.AddDataModelAsync(entityModel);

            // assert
            var testModel = await fixture.db.Recipe.FirstOrDefaultAsync(i => i.Id == entityModel.Id);
            Assert.Equal(entityModel, testModel);

            // dispose
            fixture.db.Recipe.Remove(testModel);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void RemoveDataModelAsync()
        {
            // arrange
            var entityModel = new Recipe
            {
                Id = 44440,
                Content = "Test content",
                GovermentApproval = true,
                TechApproval = false
            };
            fixture.db.Add(entityModel);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(entityModel.Id);

            // assert
            var testModel = await fixture.db.Recipe.FirstOrDefaultAsync(i => i.Id == entityModel.Id);
            Assert.Null(testModel);
        }

        [Fact]
        public async void UpdateDataModelAsync()
        {
            // arrange
            var entityModel = new Recipe
            {
                Id = 44440,
                Content = "Test content",
                GovermentApproval = true,
                TechApproval = false
            };
            fixture.db.Add(entityModel);
            await fixture.db.SaveChangesAsync();

            var updatedModel = new Recipe
            {
                Id = 44440,
                Content = "Tested content",
                GovermentApproval = true,
                TechApproval = false
            };

            // act
            await logic.UpdateDataModelAsync(updatedModel);

            // assert
            var testModel = await fixture.db.Recipe.FirstOrDefaultAsync(i => i.Id == updatedModel.Id);
            Assert.Equal(updatedModel.Content, testModel.Content);

            // dispose
            fixture.db.Recipe.Remove(testModel);
            await fixture.db.SaveChangesAsync();
        }
    }
}
