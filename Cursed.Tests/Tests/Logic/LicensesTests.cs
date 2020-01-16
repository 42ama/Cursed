using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities.Data;
using Cursed.Models.Data.Licenses;
using Cursed.Models.Data.Utility;
using Cursed.Tests.Extensions;

namespace Cursed.Tests.Tests.Logic
{
    [Collection("Tests collection")]
    public class LicensesTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly LicensesLogic logic;

        public LicensesTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new LicensesLogic(fixture.db);
        }

        public async void Dispose()
        {
            await TestsFixture.ClearDatabase(fixture.db);
        }

        private License GetLicense()
        {
            return new License
            {
                Id = 44440,
                ProductId = 44440,
                Date = DateTime.UtcNow,
                GovermentNum = 4040404
            };
        }

        private IEnumerable<ProductCatalog> GetProductsCatalog()
        {
            return new ProductCatalog[]
            {
                new ProductCatalog
                {
                    Id = 44440,
                    Cas = 4040404,
                    Name = "Testin",
                },
                new ProductCatalog
                {
                    Id = 44441,
                    Cas = 4040414,
                    Name = "Testotin",
                }
            };
        }

        private IEnumerable<License> GetLicenses()
        {
            return new License[]
            {
                new License
                {
                    Id = 44440,
                    Date = DateTime.UtcNow,
                    GovermentNum = 4040404,
                    ProductId = 44440
                },
                new License
                {
                    Id = 44441,
                    Date = DateTime.UtcNow,
                    GovermentNum = 4040414,
                    ProductId = 44440
                },
                new License
                {
                    Id = 44442,
                    Date = DateTime.UtcNow,
                    GovermentNum = 4041404,
                    ProductId = 44441
                },
            };
        }

        [Fact]
        public async void AddLicense_ToEmptyDbTable_AddedLicenseEqualExpectedLicense()
        {
            // arrange
            var expected = GetLicense();

            // act
            await logic.AddDataModelAsync(expected);

            // assert
            var actual = await fixture.db.License.FirstOrDefaultAsync(i => i.Id == expected.Id);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.ProductId, actual.ProductId);
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.GovermentNum, actual.GovermentNum);
        }

        [Fact]
        public async void RemoveLicense_FromInitializedDbTable_RemovedLicenseNotFoundInDb()
        {
            // arrange
            var license = GetLicense();

            fixture.db.Add(license);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(license.Id);

            // assert
            var actual = await fixture.db.License.FirstOrDefaultAsync(i => i.Id == license.Id);
            Assert.Null(actual);
        }

        [Fact]
        public async void UpdateLicense_AtInitializedDbTable_UpdatedLicenseEqualExpectedLicense()
        {
            // arrange
            var license = GetLicense();
            fixture.db.Add(license);
            await fixture.db.SaveChangesAsync();

            var expected = new License
            {
                Id = license.Id,
                ProductId = 44440,
                Date = DateTime.UtcNow,
                GovermentNum = 5050505
            };

            // act
            await logic.UpdateDataModelAsync(expected);

            // assert
            var actual = await fixture.db.License.FirstOrDefaultAsync(i => i.Id == expected.Id);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.ProductId, actual.ProductId);
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.GovermentNum, actual.GovermentNum);
        }

        [Fact]
        public async void GetListLicensesDataModels_FromInitializedDbTables_LogicLicensesDataModelsEqualExpectedLicensesDataModels()
        {
            // arrange
            var products = GetProductsCatalog();
            var licenses = GetLicenses();
            fixture.db.ProductCatalog.AddRange(products);
            fixture.db.License.AddRange(licenses);
            await fixture.db.SaveChangesAsync();

            var expected = new List<LicensesDataModel>
            {
                new LicensesDataModel
                {
                    Id = 44440,
                    Date = DateTime.UtcNow,
                    GovermentNum = 4040404,
                    ProductId = 44440,
                    ProductCAS = 4040404,
                    ProductName = "Testin"
                },
                new LicensesDataModel
                {
                    Id = 44441,
                    Date = DateTime.UtcNow,
                    GovermentNum = 4040414,
                    ProductId = 44440,
                    ProductCAS = 4040404,
                    ProductName = "Testin"
                },
                new LicensesDataModel
                {
                    Id = 44442,
                    Date = DateTime.UtcNow,
                    GovermentNum = 4041404,
                    ProductId = 44441,
                    ProductCAS = 4040414,
                    ProductName = "Testotin"
                }
            };

            // act
            var actual = (await logic.GetAllDataModelAsync()).ToList();

            // assert
            foreach (var expectedItem in expected)
            {
                Assert.Contains(actual, actualItem =>
                    expectedItem.Id == actualItem.Id &&
                    expectedItem.Date == actualItem.Date &&
                    expectedItem.GovermentNum == actualItem.GovermentNum &&
                    expectedItem.ProductId == actualItem.ProductId &&
                    expectedItem.ProductCAS == actualItem.ProductCAS &&
                    expectedItem.ProductName == actualItem.ProductName);
            }
        }

        [Fact]
        public async void GetLicensesDataModel_FromInitializedDbTables_LogicLicensesDataModelEqualExpectedLicensesDataModel()
        {
            // arrange
            var products = GetProductsCatalog();
            var licenses = GetLicenses();

            fixture.db.ProductCatalog.AddRange(products);
            fixture.db.License.AddRange(licenses);
            await fixture.db.SaveChangesAsync();
            var expected =  new LicensesDataModel
            {
                Id = 44441,
                Date = DateTime.UtcNow,
                GovermentNum = 4040414,
                ProductId = 44440,
                ProductCAS = 4040404,
                ProductName = "Testin"
            };

            // act
            var actual = await logic.GetSingleDataModelAsync(expected.Id);

            // assert
            Assert.Equal(actual.Id, expected.Id);
            Assert.Equal(actual.Date, expected.Date);
            Assert.Equal(actual.GovermentNum, expected.GovermentNum);
            Assert.Equal(actual.ProductId, expected.ProductId);
            Assert.Equal(actual.ProductCAS, expected.ProductCAS);
            Assert.Equal(actual.ProductName, expected.ProductName);
        }

        [Fact]
        public async void GetLicense_FromInitializedDbTable_LogicLicenseEqualExpectedLicense()
        {
            // arrange
            var expected = GetLicense();

            fixture.db.License.Add(expected);
            await fixture.db.SaveChangesAsync();

            // act
            var actual = await logic.GetSingleUpdateModelAsync(expected.Id);

            // assert
            Assert.Equal(actual.Id, expected.Id);
            Assert.Equal(actual.Date, expected.Date);
            Assert.Equal(actual.GovermentNum, expected.GovermentNum);
            Assert.Equal(actual.ProductId, expected.ProductId);
        }
    }
}
