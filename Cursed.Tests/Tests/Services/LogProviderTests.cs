using System;
using Xunit;
using System.Linq;
using Cursed.Models.Services;
using Cursed.Tests.Stubs;
using Cursed.Models.Entities.Authentication;

namespace Cursed.Tests.Tests.Services
{
    [Collection("Tests collection")]
    public class LogProviderTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly LogProvider logProvider;
        private readonly HttpContextAccessorStub httpContextAccessor;
        public LogProviderTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            httpContextAccessor = new HttpContextAccessorStub();
            logProvider = new LogProvider(fixture.dbAuth, httpContextAccessor);
        }

        public async void Dispose()
        {
            await TestsFixture.ClearDatabase(fixture.dbAuth);
        }

        private LogRecord GetLogRecord()
        {
            return new LogRecord
            {
                Record = "Record in a log",
                UserLogin = "admin",
                UserIP = "::1"
            };
        }

        private UserAuth GetUserAuth()
        {
            return new UserAuth
            {
                Login = "admin",
                PasswordHash = "hash"
            };
        }

        [Fact(Skip = "Http context accessor stub not set-uped properly")]
        public async void RecordAdded_RecordCorrect_RecordEqualRecordInDatabase()
        {
            // arrange
            var userAuth = GetUserAuth();
            fixture.dbAuth.Add(userAuth);

            var expected = GetLogRecord();
            await httpContextAccessor.SignIn("admin", "admin");
            // act
            await logProvider.AddToLogAsync("Record in a log");
            var actual = fixture.dbAuth.LogRecord.FirstOrDefault(i => i.UserLogin == expected.UserLogin);
            // assert
            Assert.Equal(expected.UserLogin, actual.UserLogin);
            Assert.Equal(expected.UserIP, actual.UserIP);
            Assert.Equal(expected.Record, actual.Record);
        }

        [Fact(Skip = "Http context accessor stub not set-uped properly")]
        public async void RecordWithUserNameAdded_RecordCorrect_RecordEqualRecordInDatabase()
        {
            // arrange
            var userAuth = GetUserAuth();
            fixture.dbAuth.Add(userAuth);

            var expected = GetLogRecord();
            // act
            await logProvider.AddToLogAsync("Record in a log", "admin");
            var actual = fixture.dbAuth.LogRecord.FirstOrDefault(i => i.UserLogin == expected.UserLogin);
            // assert
            Assert.Equal(expected.UserLogin, actual.UserLogin);
            Assert.Equal(expected.UserIP, actual.UserIP);
            Assert.Equal(expected.Record, actual.Record);
        }
    }
}
