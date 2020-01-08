using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.LogicValidation;
using Cursed.Models.Entities.Authentication;
using Cursed.Models.Data.Companies;
using Cursed.Models.Data.Utility;
using Cursed.Tests.Stubs;
using Cursed.Models.Data.Utility.ErrorHandling;
using Cursed.Models.Data.Authentication;

namespace Cursed.Tests.Tests.LogicValidation
{
    [Collection("Tests collection")]
    public class AuthenticationTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly AuthenticationLogicValidation logicValidation;

        public AuthenticationTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logicValidation = new AuthenticationLogicValidation(fixture.dbAuth, new StatusMessageFactory(), new PasswordHashStub());
        }

        public async void Dispose()
        {
            await TestsFixture.ClearDatabase(fixture.dbAuth);
        }

        private LoginModel GetLoginModel()
        {
            return new LoginModel
            {
                Login = "admin",
                Password = "tetriandoh"
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

        [Fact]
        public async void CheckLogin_EmptyDbTable_UserNotFound()
        {
            // arrange
            var loginModel = GetLoginModel();

            // act
            var statusMessage = await logicValidation.CheckLogin(loginModel);

            // assert
            Assert.False(statusMessage.IsCompleted);
            Assert.Contains(statusMessage.Problems, problem =>
                problem.Entity == "Username incorrect.");
        }

        [Fact]
        public async void CheckLoginWithWrongPassword_InitializedDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var loginModel = GetLoginModel();
            loginModel.Password = "tetridanoh";

            var expected = GetUserAuth();
            fixture.dbAuth.Add(expected);
            await fixture.dbAuth.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckLogin(loginModel);

            // assert
            Assert.False(statusMessage.IsCompleted);
            Assert.Contains(statusMessage.Problems, problem =>
                problem.Entity == "Password incorrect.");
        }

        [Fact]
        public async void CheckLoginWithRightPassword_InitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var loginModel = GetLoginModel();

            var expected = GetUserAuth();
            fixture.dbAuth.Add(expected);
            await fixture.dbAuth.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckLogin(loginModel);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }
    }
}
