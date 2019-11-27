using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities;

namespace Cursed.Tests.Tests.Logic
{
    public class FacilityTechProcesses : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture fixture;
        private readonly FacilityTechProcessesLogic logic;

        public FacilityTechProcesses(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new FacilityTechProcessesLogic(fixture.db);
        }

        [Fact]
        public async void AddDataModelAsync()
        {
            // arrange
            var entityModel = new TechProcess
            {
                FacilityId = 44440,
                RecipeId = 44440,
                DayEffiency = (decimal)12.3456
            };

            // act
            await logic.AddDataModelAsync(entityModel);

            // assert
            var testModel = await fixture.db.TechProcess.FirstOrDefaultAsync(i => i.FacilityId == entityModel.FacilityId && i.RecipeId == entityModel.RecipeId);
            Assert.Equal(entityModel, testModel);

            // dispose
            fixture.db.TechProcess.Remove(testModel);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void RemoveDataModelAsync()
        {
            // arrange
            var entityModel = new TechProcess
            {
                FacilityId = 44440,
                RecipeId = 44440,
                DayEffiency = (decimal)12.3456
            };
            fixture.db.Add(entityModel);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(entityModel);

            // assert
            var testModel = await fixture.db.TechProcess.FirstOrDefaultAsync(i => i.FacilityId == entityModel.FacilityId && i.RecipeId == entityModel.RecipeId);
            Assert.Null(testModel);
        }

        [Fact]
        public async void UpdateDataModelAsync()
        {
            // arrange
            var entityModel = new TechProcess
            {
                FacilityId = 44440,
                RecipeId = 44440,
                DayEffiency = (decimal)12.3456
            };
            fixture.db.Add(entityModel);
            await fixture.db.SaveChangesAsync();

            var updatedModel = new TechProcess
            {
                FacilityId = 44440,
                RecipeId = 44440,
                DayEffiency = (decimal)78.9012
            };

            // act
            await logic.UpdateDataModelAsync(updatedModel);

            // assert
            var testModel = await fixture.db.TechProcess.FirstOrDefaultAsync(i => i.FacilityId == entityModel.FacilityId && i.RecipeId == entityModel.RecipeId);
            Assert.Equal(updatedModel.DayEffiency, testModel.DayEffiency);

            // dispose
            fixture.db.TechProcess.Remove(testModel);
            await fixture.db.SaveChangesAsync();
        }
    }
}
