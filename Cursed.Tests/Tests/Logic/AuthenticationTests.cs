using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities.Authentication;
using Cursed.Models.Data.Companies;
using Cursed.Models.Data.Utility;
using Cursed.Tests.Extensions;

namespace Cursed.Tests.Tests.Logic
{
    [Collection("Tests collection")]
    public class AuthenticationTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly AuthenticationLogic logic;

        public AuthenticationTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new AuthenticationLogic(fixture.dbAuth);
        }

        public async void Dispose()
        {
            await TestsFixture.ClearDatabase(fixture.dbAuth);
        }

        private UserData GetUserData()
        {
            return new UserData
            {
                Login = "admin",
                RoleName = "admin"
            };
        }

        [Fact]
        public async void GetUserData_FromInitializedDbTable_ExpectedEqualActual()
        {
            // arrange
            var expected = GetUserData();
            fixture.dbAuth.Add(expected);
            await fixture.dbAuth.SaveChangesAsync();

            // act
            var actual = await logic.GetUserData(expected.Login);

            // assert
            Assert.Equal(actual.Login, expected.Login);
            Assert.Equal(actual.RoleName, expected.RoleName);
        }

        [Fact]
        public async void GetUserData_FromEmptyDbTable_ReturnsNull()
        {
            // act
            var actual = await logic.GetUserData("admin");

            // assert
            Assert.Null(actual);
        }
    }
}
