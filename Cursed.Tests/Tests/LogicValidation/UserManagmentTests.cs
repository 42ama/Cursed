using System;
using Xunit;
using Cursed.Models.LogicValidation;
using Cursed.Models.DataModel.Utility.ErrorHandling;
using Cursed.Models.Entities.Authentication;
using Cursed.Tests.Stubs;

namespace Cursed.Tests.Tests.LogicValidation
{
    [Collection("Tests collection")]
    public class UserManagmentTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly UserManagmentLogicValidation logicValidation;
        private readonly HttpContextAccessorStub contextAccessor;

        public UserManagmentTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            contextAccessor = new HttpContextAccessorStub();
            logicValidation = new UserManagmentLogicValidation(fixture.dbAuth, new StatusMessageFactory(), contextAccessor, new PasswordHashStub());
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

        private Role GetRole()
        {
            return new Role
            {
                Name = "admin"
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

        [Fact(Skip = "Context accessor SignIn method return ArgumentNull exception instead of signing in user into current user in context state.")]
        public async void CheckRemoveCurrentUser_FromInitializedDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var userData = GetUserData();
            var userAuth = GetUserAuth();
            fixture.dbAuth.Add(userData);
            fixture.dbAuth.Add(userAuth);
            await fixture.dbAuth.SaveChangesAsync();
            await contextAccessor.SignIn("admin", "admin");

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(userData.Login);

            // teardown
            await contextAccessor.SignOut();

            // assert
            Assert.False(statusMessage.IsCompleted);
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
        public async void CheckGetUserForUpdate_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var userLogin = "admin";

            // act
            var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(userLogin);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetUserForUpdate_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var userData = GetUserData();
            var userAuth = GetUserAuth();
            fixture.dbAuth.Add(userData);
            fixture.dbAuth.Add(userAuth);
            await fixture.dbAuth.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(userData.Login);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateUserAuth_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var userLogin = "admin";

            // act
            var statusMessage = await logicValidation.CheckUpdateUserAuthUpdateModelAsync(userLogin, "");

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateUserData_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var userLogin = "admin";

            // act
            var statusMessage = await logicValidation.CheckUpdateUserDataUpdateModelAsync(userLogin, "");

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateUserAuthWithIncorrectPassword_FromInitializedDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var userData = GetUserData();
            var userAuth = GetUserAuth();
            fixture.dbAuth.Add(userData);
            fixture.dbAuth.Add(userAuth);
            await fixture.dbAuth.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckUpdateUserAuthUpdateModelAsync(userAuth.Login, "teriantroh");

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateUserAuth_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var userData = GetUserData();
            var userAuth = GetUserAuth();
            fixture.dbAuth.Add(userData);
            fixture.dbAuth.Add(userAuth);
            await fixture.dbAuth.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckUpdateUserAuthUpdateModelAsync(userAuth.Login, "tetriandoh");

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateUserDataIncorrectRoleName_FromInitializedDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var userData = GetUserData();
            var userAuth = GetUserAuth();
            var role = GetRole();
            fixture.dbAuth.Add(userData);
            fixture.dbAuth.Add(userAuth);
            fixture.dbAuth.Add(role);
            await fixture.dbAuth.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckUpdateUserDataUpdateModelAsync(userData.Login, "not a role model");

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateUserData_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var userData = GetUserData();
            var userAuth = GetUserAuth();
            var role = GetRole();
            fixture.dbAuth.Add(userData);
            fixture.dbAuth.Add(userAuth);
            fixture.dbAuth.Add(role);
            await fixture.dbAuth.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckUpdateUserDataUpdateModelAsync(userData.Login, userData.RoleName);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

    }
}
