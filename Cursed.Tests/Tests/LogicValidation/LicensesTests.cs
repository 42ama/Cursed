using System;
using Xunit;
using Cursed.Models.LogicValidation;
using Cursed.Models.Entities.Data;
using Cursed.Models.DataModel.Utility.ErrorHandling;

namespace Cursed.Tests.Tests.LogicValidation
{
    [Collection("Tests collection")]
    public class LicensesTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly LicensesLogicValidation logicValidation;

        public LicensesTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logicValidation = new LicensesLogicValidation(fixture.db, new StatusMessageFactory());
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

        [Fact]
        public async void CheckRemoveLicense_FromInitializedDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var license = GetLicense();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(license.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckRemoveLicense_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var license = GetLicense();
            fixture.db.Add(license);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(license.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetLicense_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var license = GetLicense();

            // act
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(license.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetLicense_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var license = GetLicense();
            fixture.db.Add(license);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(license.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetLicenseForUpdate_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var license = GetLicense();

            // act
            var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(license.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetLicenseForUpdate_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var license = GetLicense();
            fixture.db.Add(license);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(license.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateLicense_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var license = GetLicense();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(license.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateLicense_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var license = GetLicense();
            fixture.db.Add(license);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(license.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

    }
}
