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
    public class FacilityTechProcessesTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly FacilityTechProcessesLogicValidation logicValidation;

        public FacilityTechProcessesTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logicValidation = new FacilityTechProcessesLogicValidation(fixture.db);
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

        private Facility GetFacility()
        {
            return new Facility
            {
                Id = 44440,
                Name = "Test facility #1",
                Latitude = (decimal)12.3456,
                Longitude = (decimal)78.9012
            };
        }

        private TechProcess GetTechProcess()
        {
            return new TechProcess
            {
                FacilityId = 44440,
                RecipeId = 44440,
                DayEffiency = (decimal)12.3456
            };
        }

        [Fact]
        public async void CheckRemoveTechProcess_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var techProcess = GetTechProcess();
            fixture.db.Add(techProcess);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync((techProcess.FacilityId, techProcess.RecipeId));

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckRemoveTechProcess_FromInitializedDbTableWithRelatedEntities_ErrorHandlerIsCompletedFalseHaveSpecificProblems()
        {
            // arrange
            var facility = GetFacility();
            var recipe = GetRecipe();
            var techProcess = GetTechProcess();
            fixture.db.Add(facility);
            fixture.db.Add(recipe);
            fixture.db.Add(techProcess);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync((techProcess.FacilityId, techProcess.RecipeId));

            // assert
            Assert.False(statusMessage.IsCompleted);
            Assert.Contains(statusMessage.Problems, problem =>
            problem.Entity == "Facility." && (int)problem.EntityKey == facility.Id);
            Assert.Contains(statusMessage.Problems, problem =>
            problem.Entity == "Recipe." && (int)problem.EntityKey == recipe.Id);

        }

        [Fact]
        public async void CheckUpdateTechProcess_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var techProcess = GetTechProcess();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync((techProcess.FacilityId, techProcess.RecipeId));

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateTechProcess_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var techProcess = GetTechProcess();
            fixture.db.Add(techProcess);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync((techProcess.FacilityId, techProcess.RecipeId));

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

    }
}
