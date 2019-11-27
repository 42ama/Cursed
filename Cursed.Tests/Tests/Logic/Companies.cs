using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities;

namespace Cursed.Tests.Tests.Logic
{
    public class Companies : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture fixture;
        private readonly CompaniesLogic logic;

        public Companies(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new CompaniesLogic(fixture.db);
        }

        [Fact]
        public async void AddDataModelAsync()
        {
            // arrange
            var entityModel = new Company
            {
                Id = 44440,
                Name = "Test company"
            };

            // act
            await logic.AddDataModelAsync(entityModel);
            
            // assert
            var testModel = await fixture.db.Company.FirstOrDefaultAsync(i => i.Id == entityModel.Id);
            Assert.Equal(entityModel, testModel);

            // dispose
            fixture.db.Company.Remove(testModel);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void RemoveDataModelAsync()
        {
            // arrange
            var entityModel = new Company
            {
                Id = 44440,
                Name = "Test company"
            };
            fixture.db.Add(entityModel);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(entityModel.Id);

            // assert
            var testModel = await fixture.db.Company.FirstOrDefaultAsync(i => i.Id == entityModel.Id);
            Assert.Null(testModel);
        }

        [Fact]
        public async void UpdateDataModelAsync()
        {
            // arrange
            var entityModel = new Company
            {
                Id = 44440,
                Name = "Test company"
            };
            fixture.db.Add(entityModel);
            await fixture.db.SaveChangesAsync();

            var updatedModel = new Company
            {
                Id = 44440,
                Name = "Tested company"
            };

            // act
            await logic.UpdateDataModelAsync(updatedModel);

            // assert
            var testModel = await fixture.db.Company.FirstOrDefaultAsync(i => i.Id == updatedModel.Id);
            Assert.Equal(updatedModel.Name, testModel.Name);

            // dispose
            fixture.db.Company.Remove(testModel);
            await fixture.db.SaveChangesAsync();
        }
    }
}
