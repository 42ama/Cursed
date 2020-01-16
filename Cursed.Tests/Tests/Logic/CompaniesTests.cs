using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities.Data;
using Cursed.Models.Data.Companies;
using Cursed.Models.Data.Utility;
using Cursed.Tests.Extensions;

namespace Cursed.Tests.Tests.Logic
{
    [Collection("Tests collection")]
    public class CompaniesTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly CompaniesLogic logic;

        public CompaniesTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new CompaniesLogic(fixture.db);
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

        private IEnumerable<Company> GetCompanies()
        {
            return new Company[]
            {
                new Company
                {
                    Id = 44440,
                    Name = "Test company #1",
                },
                new Company
                {
                    Id = 44441,
                    Name = "Test company #2",
                }
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
                },
                new Storage
                {
                    Id = 44442,
                    Name = "Test storage #3",
                    CompanyId = 44440
                },
                new Storage
                {
                    Id = 44443,
                    Name = "Test storage #4",
                    CompanyId = 44441
                }
            };
        }

        private IEnumerable<TransactionBatch> GetTransactions()
        {
            return new TransactionBatch[]
            {
                new TransactionBatch
                {
                    CompanyId = 44441,
                    Date = DateTime.UtcNow,
                    Id = 44440,
                    Type = "income"
                }
            };
        }


        [Fact]
        public async void AddCompany_ToEmptyDbTable_AddedCompanyEqualExpectedCompany()
        {
            // arrange
            var expected = GetCompany();

            // act
            await logic.AddDataModelAsync(expected);
            
            // assert
            var actual = await fixture.db.Company.FirstOrDefaultAsync(i => i.Id == expected.Id);
            Assert.True(actual.Id == expected.Id);
            Assert.True(actual.Name == expected.Name);
        }

        [Fact]
        public async void RemoveCompany_FromInitializedDbTable_RemovedCompanyNotFoundInDb()
        {
            // arrange
            var company = GetCompany();
            fixture.db.Add(company);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(company.Id);

            // assert
            var actual = await fixture.db.Company.FirstOrDefaultAsync(i => i.Id == company.Id);
            Assert.Null(actual);
        }

        [Fact]
        public async void UpdateCompany_AtInitializedDbTable_UpdatedCompanyEqualExpectedCompany()
        {
            // arrange
            var company = GetCompany();
            fixture.db.Add(company);
            await fixture.db.SaveChangesAsync();

            var expected = new Company
            {
                Id = company.Id,
                Name = "Tested company"
            };

            // act
            await logic.UpdateDataModelAsync(expected);

            // assert
            var actual = await fixture.db.Company.FirstOrDefaultAsync(i => i.Id == expected.Id);
            Assert.True(actual.Id == expected.Id);
            Assert.True(actual.Name == expected.Name);
        }

        [Fact]
        public async void GetListCompaniesModel_FromInitializedDbTables_LogicCompaniesModelsEqualExpectedCompaniesModels()
        {
            // arrange
            var companies = GetCompanies();
            var storages = GetStorages();
            var transactions = GetTransactions();

            fixture.db.Company.AddRange(companies);
            fixture.db.Storage.AddRange(storages);
            fixture.db.TransactionBatch.AddRange(transactions);
            await fixture.db.SaveChangesAsync();
            var expected = new List<CompaniesModel>
            {
                new CompaniesModel
                {
                    Id = 44440,
                    Name = "Test company #1",
                    StoragesCount = 3,
                    TransactionsCount = 0
                },
                new CompaniesModel
                {
                    Id = 44441,
                    Name = "Test company #2",
                    StoragesCount = 1,
                    TransactionsCount = 1,
                }
            };

            // act
            var actual = (await logic.GetAllDataModelAsync()).ToList();

            // assert
            foreach (var expectedItem in expected)
            {
                Assert.Contains(actual, actualItem => 
                    expectedItem.Id == actualItem.Id &&
                    expectedItem.Name == actualItem.Name &&
                    expectedItem.StoragesCount == actualItem.StoragesCount &&
                    expectedItem.TransactionsCount == actualItem.TransactionsCount);
            }
        }

        [Fact]
        public async void GetCompanyModel_FromInitializedDbTables_LogicCompanyModelEqualExpectedCompanyModel()
        {
            // arrange
            var companies = GetCompanies();
            var storages = GetStorages();
            var transactions = GetTransactions();

            fixture.db.Company.AddRange(companies);
            fixture.db.Storage.AddRange(storages);
            fixture.db.TransactionBatch.AddRange(transactions);
            await fixture.db.SaveChangesAsync();

            var expected = new CompanyModel
            {
                Id = 44440,
                Name = "Test company #1"
            };

            var storagesTitleIds = new List<TitleIdContainer>();
            foreach (var storage in storages.Where(i => i.CompanyId == expected.Id))
            {
                storagesTitleIds.Add(new TitleIdContainer
                {
                    Title = storage.Name,
                    Id = storage.Id
                });
            }

            expected.Storages = storagesTitleIds;

            var transactionsTitleIds = new List<TitleIdContainer>();
            foreach (var transaction in transactions.Where(i => i.CompanyId == expected.Id))
            {
                transactionsTitleIds.Add(new TitleIdContainer
                {
                    Title = transaction.Date.ToShortDateString(),
                    Id = transaction.Id
                });
            }

            expected.Transactions = transactionsTitleIds;

            // act
            var actual = await logic.GetSingleDataModelAsync(expected.Id);

            // assert
            Assert.True(actual.Id == expected.Id);
            Assert.True(actual.Name == expected.Name);
            foreach (var expectedStorage in expected.Storages)
            {
                Assert.Contains(actual.Storages, actualStorage =>
                    actualStorage.Id == expectedStorage.Id &&
                    actualStorage.Title == expectedStorage.Title);
            }
            foreach (var expectedTransaction in expected.Transactions)
            {
                Assert.Contains(actual.Transactions, actualTransaction =>
                    actualTransaction.Id == expectedTransaction.Id &&
                    actualTransaction.Title == expectedTransaction.Title);
            }
        }

        [Fact]
        public async void GetCompany_FromInitializedDbTable_LogicCompanyEqualExpectedCompany()
        {
            // arrange
            var expected =  GetCompany();

            fixture.db.Company.Add(expected);
            await fixture.db.SaveChangesAsync();

            // act
            var actual = await logic.GetSingleUpdateModelAsync(expected.Id);

            // assert
            Assert.True(actual.Id == expected.Id);
            Assert.True(actual.Name == expected.Name);
        }
    }
}
