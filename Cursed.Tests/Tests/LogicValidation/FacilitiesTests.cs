using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.LogicValidation;
using Cursed.Models.Entities.Data;
using Cursed.Models.DataModel.Facilities;
using Cursed.Models.DataModel.Shared;
using Cursed.Tests.Extensions;
using Cursed.Tests.Stubs;
using Cursed.Models.DataModel.Utility.ErrorHandling;
using Cursed.Models.Services;

namespace Cursed.Tests.Tests.LogicValidation
{
    [Collection("Tests collection")]
    public class FacilitiesTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly FacilitiesLogicValidation logicValidation;

        public FacilitiesTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logicValidation = new FacilitiesLogicValidation(fixture.db, new StatusMessageFactory());
        }

        public async void Dispose()
        {
            await TestsFixture.ClearDatabase(fixture.db);
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
        private IEnumerable<TechProcess> GetTechProcesses()
        {
            return new TechProcess[]
            {
                new TechProcess
                {
                    FacilityId = 44440,
                    RecipeId = 44440,
                    DayEfficiency = (decimal)12.3456
                },
                new TechProcess
                {
                    FacilityId = 44440,
                    RecipeId = 44441,
                    DayEfficiency = (decimal)12.3456
                }
            };
        }

        [Fact]
        public async void CheckRemoveFacility_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var facility = GetFacility();
            fixture.db.Add(facility);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(facility.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckRemoveFacility_FromInitializedDbTableWithRelatedEntities_ErrorHandlerIsCompletedFalseHaveSpecificProblems()
        {
            // arrange
            var facility = GetFacility();
            var techProcesses = GetTechProcesses();
            fixture.db.Add(facility);
            fixture.db.AddRange(techProcesses);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(facility.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
            foreach (var techProcess in techProcesses)
            {
                Assert.Contains(statusMessage.Problems, problem =>
                problem.Entity.Contains("Technological process.") && 
                Int32.Parse(problem.EntityKey) == techProcess.FacilityId);
            }
        }

        [Fact]
        public async void CheckGetFacility_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var facility = GetFacility();

            // act
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(facility.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetFacility_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var facility = GetFacility();
            fixture.db.Add(facility);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(facility.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetFacilityForUpdate_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var facility = GetFacility();

            // act
            var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(facility.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetFacilityForUpdate_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var facility = GetFacility();
            fixture.db.Add(facility);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(facility.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateFacility_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var facility = GetFacility();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(facility.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateFacility_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var facility = GetFacility();
            fixture.db.Add(facility);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(facility.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

    }
}
