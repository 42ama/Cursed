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
using Cursed.Models.Data.Utility.ErrorHandling;
using Cursed.Models.Services;
using Cursed.Models.Entities.Authentication;

namespace Cursed.Tests.Tests.LogicValidation
{
    [Collection("Tests collection")]
    public class UserManagmentTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly UserManagmentLogicValidation logicValidation;

        public UserManagmentTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logicValidation = new UserManagmentLogicValidation(fixture.dbAuth, new StatusMessageFactory());
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

        private UserAuth GetUserAuth()
        {
            return new UserAuth
            {
                Login = "admin",
                PasswordHash = "hash"
            };
        }

        [Fact]
        public async void CheckRemoveUser_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var userData = GetUserData();
            var userAuth = GetUserAuth();
            fixture.dbAuth.Add(userData);
            fixture.dbAuth.Add(userAuth);
            await fixture.dbAuth.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(userData.Login);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetUser_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var userLogin = "admin";

            // act
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(userLogin);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetUser_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var userData = GetUserData();
            var userAuth = GetUserAuth();
            fixture.dbAuth.Add(userData);
            fixture.dbAuth.Add(userAuth);
            await fixture.dbAuth.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(userData.Login);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetUserDataForUpdate_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var userLogin = "admin";

            // act
            var statusMessage = await logicValidation.CheckGetSingleUserDataUpdateModelAsync(userLogin);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetUserDataForUpdate_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var userData = GetUserData();
            var userAuth = GetUserAuth();
            fixture.dbAuth.Add(userData);
            fixture.dbAuth.Add(userAuth);
            await fixture.dbAuth.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckGetSingleUserDataUpdateModelAsync(userData.Login);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetUserAuthForUpdate_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var userLogin = "admin";

            // act
            var statusMessage = await logicValidation.CheckGetSingleUserAuthUpdateModelAsync(userLogin);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetUserAuthForUpdate_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var userData = GetUserData();
            var userAuth = GetUserAuth();
            fixture.dbAuth.Add(userData);
            fixture.dbAuth.Add(userAuth);
            await fixture.dbAuth.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckGetSingleUserAuthUpdateModelAsync(userData.Login);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateUser_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var userLogin = "admin";

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(userLogin);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateUser_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var userData = GetUserData();
            var userAuth = GetUserAuth();
            fixture.dbAuth.Add(userData);
            fixture.dbAuth.Add(userAuth);
            await fixture.dbAuth.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(userData.Login);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

    }
}
