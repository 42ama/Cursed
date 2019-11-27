using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities;

namespace Cursed.Tests.Tests.Logic
{
    public class Storages : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture fixture;
        private readonly StoragesLogic logic;

        public Storages(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new StoragesLogic(fixture.db);
        }

        [Fact]
        public async void AddDataModelAsync()
        {
            // arrange
            var entityModel = new Storage
            {
                Id = 44440,
                Name = "Test storage",
                Latitude = (decimal)12.3456,
                Longitude = (decimal)78.9012,
                CompanyId = 44440
            };

            // act
            await logic.AddDataModelAsync(entityModel);

            // assert
            var testModel = await fixture.db.Storage.FirstOrDefaultAsync(i => i.Id == entityModel.Id);
            Assert.Equal(entityModel, testModel);

            // dispose
            fixture.db.Storage.Remove(testModel);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void RemoveDataModelAsync()
        {
            // arrange
            var entityModel = new Storage
            {
                Id = 44440,
                Name = "Test storage",
                Latitude = (decimal)12.3456,
                Longitude = (decimal)78.9012,
                CompanyId = 44440
            };
            fixture.db.Add(entityModel);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(entityModel.Id);

            // assert
            var testModel = await fixture.db.Storage.FirstOrDefaultAsync(i => i.Id == entityModel.Id);
            Assert.Null(testModel);
        }

        [Fact]
        public async void UpdateDataModelAsync()
        {
            // arrange
            var entityModel = new Storage
            {
                Id = 44440,
                Name = "Test storage",
                Latitude = (decimal)12.3456,
                Longitude = (decimal)78.9012,
                CompanyId = 44440
            }; 
            fixture.db.Add(entityModel);
            await fixture.db.SaveChangesAsync();

            var updatedModel = new Storage
            {
                Id = 44440,
                Name = "Tested storage",
                Latitude = (decimal)12.3456,
                Longitude = (decimal)78.9012,
                CompanyId = 44440
            };

            // act
            await logic.UpdateDataModelAsync(updatedModel);

            // assert
            var testModel = await fixture.db.Storage.FirstOrDefaultAsync(i => i.Id == updatedModel.Id);
            Assert.Equal(updatedModel.Name, testModel.Name);

            // dispose
            fixture.db.Storage.Remove(testModel);
            await fixture.db.SaveChangesAsync();
        }
    }
}
