using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities;
using Cursed.Models.Data.Transactions;
using Cursed.Models.Data.Shared;
using Cursed.Models.Data.Utility;
using Cursed.Tests.Extensions;
using Cursed.Tests.Stubs;
using Cursed.Models.Data.Utility.ErrorHandling;

namespace Cursed.Tests.Tests.Logic
{
    [Collection("Tests collection")]
    public class TransactionsTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly TransactionsLogic logic;

        public TransactionsTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new TransactionsLogic(fixture.db, new OperationValidationStub());
        }

        public async void Dispose()
        {
            await TestsFixture.ClearDatabase(fixture.db);
        }

        private TransactionBatch GetTransaction()
        {
            return new TransactionBatch
            {
                Id = 44440,
                Date = DateTime.UtcNow,
                CompanyId = 44440,
                Type = TransactionTypes.Income,
                IsOpen = true,
                Comment = "Why chicken cross the road?"
            };
        }

        private IEnumerable<TransactionBatch> GetTransactions()
        {
            return new TransactionBatch[]
            {
                new TransactionBatch
                {
                    Id = 44440,
                    Date = DateTime.UtcNow,
                    CompanyId = 44440,
                    Type = TransactionTypes.Income,
                    IsOpen = true,
                    Comment = "Why chicken cross the road?"
                },
                new TransactionBatch
                {
                    Id = 44441,
                    Date = DateTime.UtcNow,
                    CompanyId = 44441,
                    Type = TransactionTypes.Income,
                    IsOpen = false,
                    Comment = "To get to other side : )"
                },
            };
        }
        
        private IEnumerable<Company> GetCompanies()
        {
            return new Company[]
            {
                new Company
                {
                    Id = 44440,
                    Name = "Com-pun-y #1"
                },
                new Company
                {
                    Id = 44441,
                    Name = "Company #2"
                }
            };
        }

        private IEnumerable<Operation> GetOperations()
        {
            return new Operation[]
            {
                new Operation
                {
                    TransactionId = 44440,
                    Id = 44440,
                    ProductId = 44440,
                    StorageToId = 44440,
                    StorageFromId = 44441,
                    Price = 15,
                    Quantity = 22
                },
                new Operation
                {
                    TransactionId = 44440,
                    Id = 44441,
                    ProductId = 44440,
                    StorageToId = 44440,
                    StorageFromId = 44442,
                    Price = 17,
                    Quantity = 8
                },
                new Operation
                {
                    TransactionId = 44441,
                    Id = 44442,
                    ProductId = 44440,
                    StorageToId = 44440,
                    StorageFromId = 44441,
                    Price = 15,
                    Quantity = 22
                },
            };
        }

        private IEnumerable<Product> GetProducts()
        {
            return new Product[]
            {
                new Product
                {
                    Id = 44440,
                    Uid = 44440,
                    StorageId = 44441,
                    Price = 15,
                    Quantity = 9999,
                    QuantityUnit = "mg."
                },
                new Product
                {
                    Id = 44441,
                    Uid = 44440,
                    StorageId = 44442,
                    Price = 17,
                    Quantity = 9999,
                    QuantityUnit = "mg."
                }
            };
        }

        private ProductCatalog GetProductCatalog()
        {
            return new ProductCatalog
            {
                Id = 44440,
                Cas = 4040404,
                LicenseRequired = true,
                Name = "Testotin",
                Type = ProductCatalogTypes.Product
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
                    Latitude = (decimal)12.3456,
                    Longitude = (decimal)78.9012,
                    CompanyId = 44440
                },
                new Storage
                {
                    Id = 44441,
                    Name = "Test storage #2",
                    Latitude = (decimal)12.3456,
                    Longitude = (decimal)78.9012,
                    CompanyId = 44440
                },
                new Storage
                {
                    Id = 44442,
                    Name = "Test storage #3",
                    Latitude = (decimal)12.3456,
                    Longitude = (decimal)78.9012,
                    CompanyId = 44441
                }
            };
        }

        [Fact]
        public async void OpenTransaction_AtEmptyDbTable_TransactionAtDbEqualExpectedTransaction()
        {
            // arrange
            var expected = GetTransaction();

            // act
            await logic.AddDataModelAsync(expected);

            // assert
            var actual = await fixture.db.TransactionBatch.FirstOrDefaultAsync(i => i.Id == expected.Id);
            Assert.NotNull(actual);
            Assert.Equal(expected.Comment, actual.Comment);
            Assert.Equal(expected.IsOpen, actual.IsOpen);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.CompanyId, actual.CompanyId);
            Assert.Equal(expected.Type, actual.Type);
        }

        [Fact]
        public async void RemoveOpenTransaction_AtInitializedDbTable_RemovedTransactionNotFoundInDb()
        {
            // arrange
            var transaction = GetTransaction();
            fixture.db.Add(transaction);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(transaction.Id);

            // assert
            var actual = await fixture.db.TransactionBatch.FirstOrDefaultAsync(i => i.Id == transaction.Id);
            Assert.Null(actual);
        }

        [Fact]
        public async void RemoveClosedTransaction_AtInitializedDbTable_StatusMessageFalseExpectedProblemContained()
        {
            // arrange
            var transaction = GetTransaction();
            transaction.IsOpen = false;
            fixture.db.Add(transaction);
            await fixture.db.SaveChangesAsync();
            var expectedProblem = new Problem
            {
                Entity = "Transaction open status.",
                Message = "Can't delete transaction, when it closed."
            };

            // act 
            var actual = await logic.RemoveDataModelAsync(transaction.Id);

            // assert
            Assert.False(actual.IsCompleted);
            Assert.Contains(actual.Problems, actualProblem =>
                actualProblem.Message == expectedProblem.Message &&
                actualProblem.Entity == expectedProblem.Entity);
        }

        [Fact]
        public async void RemoveOpenTransaction_AtInitializedWithOperationsDbTable_RemovedTransactionOrOpearationsNotFoundInDb()
        {
            // arrange
            var transaction = GetTransaction();
            var operations = GetOperations();
            fixture.db.Add(transaction);
            fixture.db.AddRange(operations);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(transaction.Id);

            // assert
            var actualTransaction = await fixture.db.TransactionBatch.FirstOrDefaultAsync(i => i.Id == transaction.Id);
            Assert.Null(actualTransaction);

            foreach (var operation in operations.Where(i => i.TransactionId == transaction.Id))
            {
                var actualOperation = await fixture.db.Operation.FirstOrDefaultAsync(i => i.Id == operation.Id);
                Assert.Null(actualOperation);
            }
        }

        [Fact]
        public async void UpdateOpenTransaction_AtInitializedDbTable_UpdatedTransactionEqualExpectedTransaction()
        {
            // arrange
            var transaction = GetTransaction();
            fixture.db.Add(transaction);
            await fixture.db.SaveChangesAsync();

            var expected = new TransactionBatch
            {
                Id = transaction.Id,
                Date = DateTime.UtcNow,
                CompanyId = 44440,
                Type = TransactionTypes.Income,
                IsOpen = true,
                Comment = "Cause nest is on the other side."
            };

            // act
            await logic.UpdateDataModelAsync(expected);

            // assert
            var actual = await fixture.db.TransactionBatch.FirstOrDefaultAsync(i => i.Id == transaction.Id);
            Assert.NotNull(actual);
            Assert.Equal(expected.Comment, actual.Comment);
            Assert.Equal(expected.IsOpen, actual.IsOpen);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.CompanyId, actual.CompanyId);
            Assert.Equal(expected.Type, actual.Type);
        }

        [Fact]
        public async void UpdateClosedTransaction_AtInitializedDbTable_StatusMessageFalseExpectedProblemContained()
        {
            // arrange
            var transaction = GetTransaction();
            transaction.IsOpen = false;
            fixture.db.Add(transaction);
            await fixture.db.SaveChangesAsync();

            var expected = new TransactionBatch
            {
                Id = transaction.Id,
                Date = DateTime.UtcNow,
                CompanyId = 44440,
                Type = TransactionTypes.Income,
                IsOpen = false,
                Comment = "Cause nest is on the other side."
            };
            var expectedProblem = new Problem
            {
                Entity = "Transaction open status.",
                Message = "Can't update transaction, when it closed."
            };

            // act 
            var actual = await logic.UpdateDataModelAsync(expected);

            // assert
            Assert.False(actual.IsCompleted);
            Assert.Contains(actual.Problems, actualProblem =>
                actualProblem.Message == expectedProblem.Message &&
                actualProblem.Entity == expectedProblem.Entity);
        }

        [Fact]
        public async void CloseTransaction_AtInitializedDbTable_DataAtDbEqualExpected()
        {
            // arrange
            var transaction = GetTransaction();
            var operations = GetOperations();
            var productsCatalog = GetProductCatalog();
            var products = GetProducts();
            var storages = GetStorages();
            fixture.db.Add(transaction);
            fixture.db.AddRange(productsCatalog);
            fixture.db.AddRange(products);
            fixture.db.AddRange(storages);
            fixture.db.AddRange(operations);
            await fixture.db.SaveChangesAsync();

            var expected = new Product
            {
                Id = 0,
                Uid = 44440,
                StorageId = 44440,
                Price = 0,
                Quantity = 30,
                QuantityUnit = "mg."
            };

            // act
            await logic.CloseTransactionAsync(transaction.Id);

            // assert
            var actual = await fixture.db.Product.FirstOrDefaultAsync(i => i.Uid == expected.Uid && i.StorageId == expected.StorageId);
            Assert.NotNull(actual);
            Assert.Equal(expected.Quantity, actual.Quantity);
        }

        [Fact]
        public async void GetListTransactionsModel_FromInitializedDbTables_LogicTransactionsModelsEqualExpectedTransactionsModels()
        {
            // arrange
            var transactions = GetTransactions();
            var companies = GetCompanies();
            var operations = GetOperations();
            fixture.db.AddRange(transactions);
            fixture.db.AddRange(companies);
            fixture.db.AddRange(operations);
            await fixture.db.SaveChangesAsync();

            var expected = new TransactionsModel[]
            {
                new TransactionsModel
                {
                    Id = 44440,
                    Date = DateTime.UtcNow,
                    CompanyId = 44440,
                    Type = TransactionTypes.Income,
                    IsOpen = true,
                    Comment = "Why chicken cross the road?",
                    CompanyName = "Com-pun-y #1",
                    OperationsCount = 2
                },
                new TransactionsModel
                {
                    Id = 44441,
                    Date = DateTime.UtcNow,
                    CompanyId = 44441,
                    Type = TransactionTypes.Income,
                    IsOpen = false,
                    Comment = "To get to other side : )",
                    CompanyName = "Company #2",
                    OperationsCount = 1
                },
            };

            // act
            var actual = (await logic.GetAllDataModelAsync()).ReturnValue.ToList();

            // assert
            foreach (var expectedItem in expected)
            {
                Assert.Contains(actual, actualItem =>
                    expectedItem.Id == actualItem.Id &&
                    expectedItem.Date == actualItem.Date &&
                    expectedItem.CompanyId == actualItem.CompanyId &&
                    expectedItem.Type == actualItem.Type &&
                    expectedItem.IsOpen == actualItem.IsOpen &&
                    expectedItem.Comment == actualItem.Comment &&
                    expectedItem.CompanyName == actualItem.CompanyName &&
                    expectedItem.OperationsCount == actualItem.OperationsCount);
            }
        }

        [Fact]
        public async void GetTransactionModel_FromInitializedDbTables_LogicTransactionModelEqualExpectedTransactionModel()
        {
            // arrange
            var transactions = GetTransactions();
            var companies = GetCompanies();
            var operations = GetOperations();
            fixture.db.AddRange(transactions);
            fixture.db.AddRange(companies);
            fixture.db.AddRange(operations);
            await fixture.db.SaveChangesAsync();

            var expected = new TransactionModel
            {
                Id = 44440,
                Date = DateTime.UtcNow,
                CompanyId = 44440,
                Type = TransactionTypes.Income,
                IsOpen = true,
                Comment = "Why chicken cross the road?",
                CompanyName = "Com-pun-y #1",
                Operations = new List<Operation>()
            };
            foreach (var operation in operations.Where(i => i.TransactionId == expected.Id))
            {
                expected.Operations.Add(operation);
            }

            // act
            var actual = (await logic.GetSingleDataModelAsync(expected.Id)).ReturnValue;


            // assert
            Assert.NotNull(actual);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.CompanyId, actual.CompanyId);
            Assert.Equal(expected.Type, actual.Type);
            Assert.Equal(expected.IsOpen, actual.IsOpen);
            Assert.Equal(expected.Comment, actual.Comment);
            Assert.Equal(expected.CompanyName, actual.CompanyName);
            foreach (var expectedOperation in expected.Operations)
            {
                Assert.Contains(actual.Operations, actualOperation =>
                    expectedOperation.TransactionId == actualOperation.TransactionId &&
                    expectedOperation.Id == actualOperation.Id &&
                    expectedOperation.ProductId == actualOperation.ProductId &&
                    expectedOperation.StorageToId == actualOperation.StorageToId &&
                    expectedOperation.StorageFromId == actualOperation.StorageFromId &&
                    expectedOperation.Price == actualOperation.Price &&
                    expectedOperation.Quantity == actualOperation.Quantity);
            }
        }

        [Fact]
        public async void GetTransactionBatch_FromInitializedDbTable_LogicTransactionBatchEqualExpectedTransactionBatch()
        {
            // arrange
            var expected = GetTransaction();

            fixture.db.Add(expected);
            await fixture.db.SaveChangesAsync();

            // act
            var actual = (await logic.GetSingleUpdateModelAsync(expected.Id)).ReturnValue;

            // assert
            Assert.NotNull(actual);
            Assert.Equal(expected.Comment, actual.Comment);
            Assert.Equal(expected.IsOpen, actual.IsOpen);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.CompanyId, actual.CompanyId);
            Assert.Equal(expected.Type, actual.Type);
        }
    }
}
