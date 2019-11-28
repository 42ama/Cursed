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
            var expected = new TechProcess
            {
                FacilityId = 44440,
                RecipeId = 44440,
                DayEffiency = (decimal)12.3456
            };

            // act
            await logic.AddDataModelAsync(expected);

            // assert
            var actual = await fixture.db.TechProcess.FirstOrDefaultAsync(i => i.FacilityId == expected.FacilityId && i.RecipeId == expected.RecipeId);
            Assert.Equal(expected.FacilityId, actual.FacilityId);
            Assert.Equal(expected.RecipeId, actual.RecipeId);
            Assert.Equal(expected.DayEffiency, actual.DayEffiency);
            // dispose
            fixture.db.TechProcess.Remove(actual);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void RemoveDataModelAsync()
        {
            // arrange
            var techProcess = new TechProcess
            {
                FacilityId = 44440,
                RecipeId = 44440,
                DayEffiency = (decimal)12.3456
            };
            fixture.db.Add(techProcess);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(techProcess);

            // assert
            var actual = await fixture.db.TechProcess.FirstOrDefaultAsync(i => i.FacilityId == techProcess.FacilityId && i.RecipeId == techProcess.RecipeId);
            Assert.Null(actual);
        }

        [Fact]
        public async void UpdateDataModelAsync()
        {
            // arrange
            var techProcess = new TechProcess
            {
                FacilityId = 44440,
                RecipeId = 44440,
                DayEffiency = (decimal)12.3456
            };
            fixture.db.Add(techProcess);
            await fixture.db.SaveChangesAsync();

            var expected = new TechProcess
            {
                FacilityId = 44440,
                RecipeId = 44440,
                DayEffiency = (decimal)78.9012
            };

            // act
            await logic.UpdateDataModelAsync(expected);

            // assert
            var actual = await fixture.db.TechProcess.FirstOrDefaultAsync(i => i.FacilityId == techProcess.FacilityId && i.RecipeId == techProcess.RecipeId);
            Assert.Equal(expected.FacilityId, actual.FacilityId);
            Assert.Equal(expected.RecipeId, actual.RecipeId);
            Assert.Equal(expected.DayEffiency, actual.DayEffiency);

            // dispose
            fixture.db.TechProcess.Remove(actual);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void GetAllDataModelAsync()
        {
            int facilityId = 44440;
            var facilities = new Facility[]
            {
                new Facility
                {
                    Id = facilityId,
                    Name = "Test facility"
                }
            };
            var recipes = new Recipe[]
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
            var techProcesses = new TechProcess[]
            {
                new TechProcess
                {
                    FacilityId = facilityId,
                    RecipeId = 44440,
                    DayEffiency = 41
                },
                new TechProcess
                {
                    FacilityId = facilityId,
                    RecipeId = 44441,
                    DayEffiency = 42
                },
                new TechProcess
                {
                    FacilityId = facilityId,
                    RecipeId = 44442,
                    DayEffiency = 43
                }
            };
            fixture.db.Facility.AddRange(facilities);
            fixture.db.Recipe.AddRange(recipes);
            fixture.db.TechProcess.AddRange(techProcesses);
            await fixture.db.SaveChangesAsync();

            var expected = new List<FacilityTechProcessesDataModel>
            {
                new FacilityTechProcessesDataModel
                {
                    FacilityId = facilityId,
                    FacilityName = "Test facility",
                    DayEffiency = 41,
                    RecipeId = 44440,
                    RecipeContent = "Im recipe #1 content",
                    RecipeGovApprov = true,
                    RecipeTechApprov = true
                },
                new FacilityTechProcessesDataModel
                {
                    FacilityId = facilityId,
                    FacilityName = "Test facility",
                    DayEffiency = 42,
                    RecipeId = 44441,
                    RecipeContent = "Im recipe #2 content",
                    RecipeGovApprov = true,
                    RecipeTechApprov = false
                },
                new FacilityTechProcessesDataModel
                {
                    FacilityId = facilityId,
                    FacilityName = "Test facility",
                    DayEffiency = 43,
                    RecipeId = 44442,
                    RecipeContent = "Im recipe #3 content",
                    RecipeGovApprov = false,
                    RecipeTechApprov = true
                }
            };

            // act
            var actual = (await logic.GetAllDataModelAsync(facilityId)).ToList();

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

            // dispose
            fixture.db.TechProcess.RemoveRange(techProcesses);
            fixture.db.Facility.RemoveRange(facilities);
            fixture.db.Recipe.RemoveRange(recipes);
            await fixture.db.SaveChangesAsync();
        }
    }
}
