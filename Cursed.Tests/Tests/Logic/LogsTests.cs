using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities;
using Cursed.Models.Data.Companies;
using Cursed.Models.Data.Utility;
using Cursed.Tests.Extensions;
using Cursed.Models.Entities.Authentication;

namespace Cursed.Tests.Tests.Logic
{
    [Collection("Tests collection")]
    public class LogsTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly LogsLogic logic;

        public LogsTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new LogsLogic(fixture.dbAuth);
        }

        public async void Dispose()
        {
            await TestsFixture.ClearDatabase(fixture.db);
        }

        private LogRecord[] GetLogRecords()
        {
            return new LogRecord[]
            {
                new LogRecord
                {
                    UserLogin = "admin",
                    UserIP = "::1",
                    Record = "Action #1"
                },
                new LogRecord
                {
                    UserLogin = "admin",
                    UserIP = "::1",
                    Record = "Action #2"
                },
                new LogRecord
                {
                    UserLogin = "manager",
                    UserIP = "::2",
                    Record = "Action #3"
                },
            };
        }

        [Fact]
        public async void GetListLogRecords_FromInitializedDbTables_LogicLogRecordsEqualExpectedLogRecords()
        {
            // arrange
            var logRecords = GetLogRecords();
            fixture.dbAuth.LogRecord.AddRange(logRecords);
            await fixture.dbAuth.SaveChangesAsync();
            var expected = logRecords;
            // act
            var actual = (await logic.GetAllDataModelAsync()).ToList();

            // assert
            foreach (var expectedItem in expected)
            {
                Assert.Contains(actual, actualItem =>
                    expectedItem.UserLogin == actualItem.UserLogin &&
                    expectedItem.UserIP == actualItem.UserIP &&
                    expectedItem.Record == actualItem.Record);
            }
        }
    }
}
