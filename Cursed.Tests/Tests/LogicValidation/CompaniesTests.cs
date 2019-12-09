using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.LogicValidation;
using Cursed.Models.Entities;
using Cursed.Models.Data.Companies;
using Cursed.Models.Data.Utility;
using Cursed.Tests.Extensions;

namespace Cursed.Tests.Tests.LogicValidation
{
    [Collection("Tests collection")]
    public class CompaniesTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly CompaniesLogicValidation logicValidation;

        public CompaniesTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logicValidation = new CompaniesLogicValidation(fixture.db);
        }

        public async void Dispose()
        {
            await TestsFixture.ClearDatabase(fixture.db);
        }

        private Company GetCompany()
        {
            return new Company
            {
                Id = 44440,
                Name = "Test company"
            };
        }

        private IEnumerable<Storage> GetStorages()
        {
            return new Storage[]
            {
                new Storage
                {
                    Id = 44440,
                    Name = "Test storage #1",
                    CompanyId = 44440
                },
                new Storage
                {
                    Id = 44441,
                    Name = "Test storage #2",
                    CompanyId = 44440
                }
            };
        }

        private TransactionBatch GetTransaction()
        {
            return new TransactionBatch
            {
                CompanyId = 44440,
                Date = DateTime.UtcNow,
                Id = 44440,
                Type = "income"
            };
        }

        [Fact]
        public async void CheckRemoveCompany_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var company = GetCompany();
            fixture.db.Add(company);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(company.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckRemoveCompany_FromInitializedDbTableWithRelatedEntities_ErrorHandlerIsCompletedFalseHaveSpecificProblems()
        {
            // arrange
            var company = GetCompany();
            var transaction = GetTransaction();
            var storages = GetStorages();
            fixture.db.Add(company);
            fixture.db.Add(transaction);
            fixture.db.AddRange(storages);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(company.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
            foreach (var storage in storages)
            {
                Assert.Contains(statusMessage.Problems, problem =>
                problem.Entity == "Storage." && (int)problem.EntityKey == storage.Id);
            }
            Assert.Contains(statusMessage.Problems, problem =>
            problem.Entity == "Transaction." && (int)problem.EntityKey == transaction.Id);
        }

        [Fact]
        public async void CheckGetCompany_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var company = GetCompany();

            // act
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(company.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetCompany_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var company = GetCompany();
            fixture.db.Add(company);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(company.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetCompanyForUpdate_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var company = GetCompany();

            // act
            var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(company.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetCompanyForUpdate_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var company = GetCompany();
            fixture.db.Add(company);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(company.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateCompany_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var company = GetCompany();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(company.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateCompany_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var company = GetCompany();
            fixture.db.Add(company);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(company.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

    }
}
