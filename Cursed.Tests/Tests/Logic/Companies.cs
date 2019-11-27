using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities;
using Cursed.Models.Data.Companies;
using Cursed.Models.Data.Utility;

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
            var expected = new Company
            {
                Id = 44440,
                Name = "Test company"
            };

            // act
            await logic.AddDataModelAsync(expected);
            
            // assert
            var actual = await fixture.db.Company.FirstOrDefaultAsync(i => i.Id == expected.Id);
            Assert.True(actual.Id == expected.Id);
            Assert.True(actual.Name == expected.Name);

            // dispose
            fixture.db.Company.Remove(actual);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void RemoveDataModelAsync()
        {
            // arrange
            var company = new Company
            {
                Id = 44440,
                Name = "Test company"
            };
            fixture.db.Add(company);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(company.Id);

            // assert
            var actual = await fixture.db.Company.FirstOrDefaultAsync(i => i.Id == company.Id);
            Assert.Null(actual);
        }

        [Fact]
        public async void UpdateDataModelAsync()
        {
            // arrange
            var company = new Company
            {
                Id = 44440,
                Name = "Test company"
            };
            fixture.db.Add(company);
            await fixture.db.SaveChangesAsync();

            var expected = new Company
            {
                Id = 44440,
                Name = "Tested company"
            };

            // act
            await logic.UpdateDataModelAsync(expected);

            // assert
            var actual = await fixture.db.Company.FirstOrDefaultAsync(i => i.Id == expected.Id);
            Assert.True(actual.Id == expected.Id);
            Assert.True(actual.Name == expected.Name);

            // dispose
            fixture.db.Company.Remove(actual);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void GetAllDataModelAsync()
        {
            // arrange
            var companies = new Company[]
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

            var storages = new Storage[]
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
            var transactions = new TransactionBatch[]
            {
                new TransactionBatch
                {
                    CompanyId = 44441,
                    Date = DateTime.Now,
                    Id = 44440,
                    Type = "income"
                }
            };
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

            // dispose
            fixture.db.Storage.RemoveRange(storages);
            fixture.db.TransactionBatch.RemoveRange(transactions);
            fixture.db.Company.RemoveRange(companies);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void GetSingleDataModelAsync()
        {
            // arrange
            var companies = new Company[]
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

            var storages = new Storage[]
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
            var transactions = new TransactionBatch[]
            {
                new TransactionBatch
                {
                    CompanyId = 44441,
                    Date = DateTime.Now,
                    Id = 44440,
                    Type = "income"
                }
            };

            fixture.db.Company.AddRange(companies);
            fixture.db.Storage.AddRange(storages);
            fixture.db.TransactionBatch.AddRange(transactions);
            await fixture.db.SaveChangesAsync();

            int companyId = 44440;

            var expected = new CompanyModel
            {
                Id = companyId,
                Name = "Test company #1"
            };

            var storagesTitleIds = new List<TitleIdContainer>();
            foreach (var storage in storages.Where(i => i.CompanyId == companyId))
            {
                storagesTitleIds.Add(new TitleIdContainer
                {
                    Title = storage.Name,
                    Id = storage.Id
                });
            }

            expected.Storages = storagesTitleIds;

            var transactionsTitleIds = new List<TitleIdContainer>();
            foreach (var transaction in transactions.Where(i => i.CompanyId == companyId))
            {
                transactionsTitleIds.Add(new TitleIdContainer
                {
                    Title = transaction.Date.ToShortDateString(),
                    Id = transaction.Id
                });
            }

            expected.Transactions = transactionsTitleIds;

            // act
            var actual = await logic.GetSingleDataModelAsync(companyId);

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

            // dispose
            fixture.db.Storage.RemoveRange(storages);
            fixture.db.TransactionBatch.RemoveRange(transactions);
            fixture.db.Company.RemoveRange(companies);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void GetSingleUpdateModelAsync()
        {
            // arrange
            var expected =  new Company
            {
                Id = 44440,
                Name = "Test company #1",
            };

            fixture.db.Company.Add(expected);
            await fixture.db.SaveChangesAsync();

            // act
            var actual = await logic.GetSingleUpdateModelAsync(expected.Id);

            // assert
            Assert.True(actual.Id == expected.Id);
            Assert.True(actual.Name == expected.Name);
            
            // dispose
            fixture.db.Company.Remove(actual);
            await fixture.db.SaveChangesAsync();
        }
    }
}
