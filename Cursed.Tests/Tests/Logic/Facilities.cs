using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities;

namespace Cursed.Tests.Tests.Logic
{
    public class Facilities : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture fixture;
        private readonly FacilitiesLogic logic;

        public Facilities(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new FacilitiesLogic(fixture.db);
        }

        [Fact]
        public async void AddDataModelAsync()
        {
            // arrange
            var entityModel = new Facility
            {
                Id = 44440,
                Name = "Test facility",
                Latitude = (decimal)12.3456,
                Longitude = (decimal)78.9012
            };

            // act
            await logic.AddDataModelAsync(entityModel);

            // assert
            var testModel = await fixture.db.Facility.FirstOrDefaultAsync(i => i.Id == entityModel.Id);
            Assert.Equal(entityModel, testModel);

            // dispose
            fixture.db.Facility.Remove(testModel);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void RemoveDataModelAsync()
        {
            // arrange
            var entityModel = new Facility
            {
                Id = 44440,
                Name = "Test facility",
                Latitude = (decimal)12.3456,
                Longitude = (decimal)78.9012
            };
            fixture.db.Add(entityModel);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(entityModel.Id);

            // assert
            var testModel = await fixture.db.Facility.FirstOrDefaultAsync(i => i.Id == entityModel.Id);
            Assert.Null(testModel);
        }

        [Fact]
        public async void UpdateDataModelAsync()
        {
            // arrange
            var entityModel = new Facility
            {
                Id = 44440,
                Name = "Test facility",
                Latitude = (decimal)12.3456,
                Longitude = (decimal)78.9012
            }; 
            fixture.db.Add(entityModel);
            await fixture.db.SaveChangesAsync();

            var updatedModel = new Facility
            {
                Id = 44440,
                Name = "Tested facility",
                Latitude = (decimal)12.3456,
                Longitude = (decimal)78.9012
            };

            // act
            await logic.UpdateDataModelAsync(updatedModel);

            // assert
            var testModel = await fixture.db.Facility.FirstOrDefaultAsync(i => i.Id == updatedModel.Id);
            Assert.Equal(updatedModel.Name, testModel.Name);

            // dispose
            fixture.db.Facility.Remove(testModel);
            await fixture.db.SaveChangesAsync();
        }
    }
}
