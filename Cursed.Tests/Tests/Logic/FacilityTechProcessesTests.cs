using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities;
using Cursed.Models.Data.FacilityTechProcesses;

namespace Cursed.Tests.Tests.Logic
{
    [Collection("Tests collection")]
    public class FacilityTechProcessesTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly FacilityTechProcessesLogic logic;

        public FacilityTechProcessesTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new FacilityTechProcessesLogic(fixture.db);
        }

        public async void Dispose()
        {
            await TestsFixture.ClearDatabase(fixture.db);
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

        private IEnumerable<Facility> GetFacilities()
        {
            return new Facility[]
            {
                new Facility
                {
                    Id = 44440,
                    Name = "Test facility"
                }
            };
        }

        private IEnumerable<Recipe> GetRecipes()
        {
            return new Recipe[]
            {
                new Recipe
                {
                    Id = 44440,
                    Content = "Im recipe #1 content",
                    GovermentApproval = true,
                    TechApproval = true
                },
                new Recipe
                {
                    Id = 44441,
                    Content = "Im recipe #2 content",
                    GovermentApproval = true,
                    TechApproval = null
                },
                new Recipe
                {
                    Id = 44442,
                    Content = "Im recipe #3 content",
                    GovermentApproval = false,
                    TechApproval = true
                }
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
                    DayEffiency = 41
                },
                new TechProcess
                {
                    FacilityId = 44440,
                    RecipeId = 44441,
                    DayEffiency = 42
                },
                new TechProcess
                {
                    FacilityId = 44440,
                    RecipeId = 44442,
                    DayEffiency = 43
                }
            };
        }

        [Fact]
        public async void AddTechProcess_ToEmptyDbTable_AddedTechProcessEqualExpectedTechProcess()
        {
            // arrange
            var expected = GetTechProcess();

            // act
            await logic.AddDataModelAsync(expected);

            // assert
            var actual = await fixture.db.TechProcess.FirstOrDefaultAsync(i => i.FacilityId == expected.FacilityId && i.RecipeId == expected.RecipeId);
            Assert.Equal(expected.FacilityId, actual.FacilityId);
            Assert.Equal(expected.RecipeId, actual.RecipeId);
            Assert.Equal(expected.DayEffiency, actual.DayEffiency);
        }

        [Fact]
        public async void RemoveTechProcess_FromInitializedDbTable_RemovedTechProcessNotFoundInDb()
        {
            // arrange
            var techProcess = GetTechProcess();
            fixture.db.Add(techProcess);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(techProcess);

            // assert
            var actual = await fixture.db.TechProcess.FirstOrDefaultAsync(i => i.FacilityId == techProcess.FacilityId && i.RecipeId == techProcess.RecipeId);
            Assert.Null(actual);
        }

        [Fact]
        public async void UpdateTechProcess_AtInitializedDbTable_UpdatedTechProcessEqualExpectedTechProcess()
        {
            // arrange
            var techProcess = GetTechProcess();
            fixture.db.Add(techProcess);
            await fixture.db.SaveChangesAsync();

            var expected = new TechProcess
            {
                FacilityId = techProcess.FacilityId,
                RecipeId = techProcess.RecipeId,
                DayEffiency = (decimal)78.9012
            };

            // act
            await logic.UpdateDataModelAsync(expected);

            // assert
            var actual = await fixture.db.TechProcess.FirstOrDefaultAsync(i => i.FacilityId == techProcess.FacilityId && i.RecipeId == techProcess.RecipeId);
            Assert.Equal(expected.FacilityId, actual.FacilityId);
            Assert.Equal(expected.RecipeId, actual.RecipeId);
            Assert.Equal(expected.DayEffiency, actual.DayEffiency);
        }

        [Fact]
        public async void GetListFacilityTechProcessesDataModel_FromInitializedDbTables_LogicFacilityTechProcessesDataModelsEqualExpectedFacilityTechProcessesDataModels()
        {
            var facilities = GetFacilities();
            var recipes = GetRecipes();
            var techProcesses = GetTechProcesses();
            fixture.db.Facility.AddRange(facilities);
            fixture.db.Recipe.AddRange(recipes);
            fixture.db.TechProcess.AddRange(techProcesses);
            await fixture.db.SaveChangesAsync();

            var expected = new List<FacilityTechProcessesDataModel>
            {
                new FacilityTechProcessesDataModel
                {
                    FacilityId = 44440,
                    FacilityName = "Test facility",
                    DayEffiency = 41,
                    RecipeId = 44440,
                    RecipeContent = "Im recipe #1 content",
                    RecipeGovApprov = true,
                    RecipeTechApprov = true
                },
                new FacilityTechProcessesDataModel
                {
                    FacilityId = 44440,
                    FacilityName = "Test facility",
                    DayEffiency = 42,
                    RecipeId = 44441,
                    RecipeContent = "Im recipe #2 content",
                    RecipeGovApprov = true,
                    RecipeTechApprov = false
                },
                new FacilityTechProcessesDataModel
                {
                    FacilityId = 44440,
                    FacilityName = "Test facility",
                    DayEffiency = 43,
                    RecipeId = 44442,
                    RecipeContent = "Im recipe #3 content",
                    RecipeGovApprov = false,
                    RecipeTechApprov = true
                }
            };

            // act
            var actual = (await logic.GetAllDataModelAsync(expected.First().FacilityId)).ToList();

            // assert
            foreach (var expectedItem in expected)
            {
                Assert.Contains(actual, actualItem =>
                    expectedItem.RecipeId == actualItem.RecipeId &&
                    expectedItem.FacilityId == actualItem.FacilityId &&
                    expectedItem.FacilityName == actualItem.FacilityName &&
                    expectedItem.DayEffiency == actualItem.DayEffiency &&
                    expectedItem.RecipeContent == actualItem.RecipeContent &&
                    expectedItem.RecipeGovApprov == actualItem.RecipeGovApprov &&
                    expectedItem.RecipeTechApprov == actualItem.RecipeTechApprov);
            }
        }
    }
}
