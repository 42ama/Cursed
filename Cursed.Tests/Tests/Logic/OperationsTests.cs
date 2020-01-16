using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities.Data;
using Cursed.Models.DataModel.Shared;

namespace Cursed.Tests.Tests.Logic
{
    [Collection("Tests collection")]
    public class OperationsTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly OperationsLogic logic;

        public OperationsTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new OperationsLogic(fixture.db);
        }

        public async void Dispose()
        {
            await TestsFixture.ClearDatabase(fixture.db);
        }

        private Operation GetOperation()
        {
            return new Operation
            {
                Id = 44440,
                ProductId = 44440,
                Price = 12,
                Quantity = 14,
                StorageFromId = 44440,
                StorageToId = 44441,
                TransactionId = 44440
            };
        }

        [Fact]
        public async void AddOperation_ToEmptyDbTable_AddedOperationEqualExpectedOperation()
        {
            // arrange
            var expected = GetOperation();

            // act
            await logic.AddDataModelAsync(expected);

            // assert
            var actual = await fixture.db.Operation.FirstOrDefaultAsync(i => i.Id == expected.Id);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Price, actual.Price);
            Assert.Equal(expected.ProductId, actual.ProductId);
            Assert.Equal(expected.Quantity, actual.Quantity);
            Assert.Equal(expected.StorageFromId, actual.StorageFromId);
            Assert.Equal(expected.StorageToId, actual.StorageToId);
            Assert.Equal(expected.TransactionId, actual.TransactionId);
        }

        [Fact]
        public async void RemoveOperation_FromInitializedDbTable_RemovedOperationNotFoundInDb()
        {
            // arrange
            var operation = GetOperation();
            fixture.db.Add(operation);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(operation);

            // assert
            var actual = await fixture.db.Operation.FirstOrDefaultAsync(i => i.Id == operation.Id);
            Assert.Null(actual);
        }

        [Fact]
        public async void UpdateOperation_AtInitializedDbTable_UpdatedOperationEqualExpectedOperation()
        {
            // arrange
            var operation = GetOperation();
            fixture.db.Add(operation);
            await fixture.db.SaveChangesAsync();

            var expected = new Operation
            {
                Id = 44440,
                ProductId = 44440,
                Price = 220,
                Quantity = 220,
                StorageFromId = 44440,
                StorageToId = 44441,
                TransactionId = 44440
            };

            // act
            await logic.UpdateDataModelAsync(expected);

            // assert
            var actual = await fixture.db.Operation.FirstOrDefaultAsync(i => i.Id == expected.Id);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Price, actual.Price);
            Assert.Equal(expected.ProductId, actual.ProductId);
            Assert.Equal(expected.Quantity, actual.Quantity);
            Assert.Equal(expected.StorageFromId, actual.StorageFromId);
            Assert.Equal(expected.StorageToId, actual.StorageToId);
            Assert.Equal(expected.TransactionId, actual.TransactionId);
        }

        [Fact]
        public async void GetOperation_FromInitializedDbTable_LogicOperationEqualExpectedOperation()
        {
            // arrange
            var expected = GetOperation();

            fixture.db.Add(expected);
            await fixture.db.SaveChangesAsync();

            // act
            var actual = await logic.GetSingleUpdateModelAsync(expected.Id);

            // assert
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Price, actual.Price);
            Assert.Equal(expected.ProductId, actual.ProductId);
            Assert.Equal(expected.Quantity, actual.Quantity);
            Assert.Equal(expected.StorageFromId, actual.StorageFromId);
            Assert.Equal(expected.StorageToId, actual.StorageToId);
            Assert.Equal(expected.TransactionId, actual.TransactionId);
        }
    }
}
