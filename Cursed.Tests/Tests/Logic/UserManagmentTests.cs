using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities.Authentication;
using Cursed.Models.Data.Companies;
using Cursed.Models.Data.Utility;
using Cursed.Tests.Stubs;
using Cursed.Models.Data.Authentication;

namespace Cursed.Tests.Tests.Logic
{
    [Collection("Tests collection")]
    public class UserManagmentTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly UserManagmentLogic logic;

        public UserManagmentTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new UserManagmentLogic(fixture.dbAuth, new PasswordHashStub());
        }

        public async void Dispose()
        {
            await TestsFixture.ClearDatabase(fixture.dbAuth);
        }

        private RegistrationModel GetRegistrationModel()
        {
            return new RegistrationModel
            {
                Login = "admin",
                Password = "safeWord",
                PasswordConfirm = "safeWord",
                RoleName = "admin"
            };
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

        private IList<UserData> GetUsersData()
        {
            return new UserData[]
            {
                new UserData
                {
                    Login = "admin",
                    RoleName = "admin"
                },
                new UserData
                {
                    Login = "technologist",
                    RoleName = "technologist"
                },
                new UserData
                {
                    Login = "seniorTechnologist",
                    RoleName = "seniorTechnologist"
                },
            };
        }

        [Fact]
        public async void AddUser_ToEmptyDbTable_AddedUserEqualExpectedUser()
        {
            // arrange
            var expected = GetRegistrationModel();

            // act
            await logic.AddDataModelAsync(expected);

            // assert
            var actualUserData = await fixture.dbAuth.UserData.FirstOrDefaultAsync(i => i.Login == expected.Login);
            var actualUserAuth = await fixture.dbAuth.UserAuth.FirstOrDefaultAsync(i => i.Login == expected.Login);
            Assert.Equal(expected.Login, actualUserData.Login);
            Assert.Equal(expected.RoleName, actualUserData.RoleName);
            Assert.Equal(expected.Login, actualUserAuth.Login);
        }

        [Fact]
        public async void RemoveUser_FromInitializedDbTable_RemovedUserNotFoundInDb()
        {
            // arrange
            var userData = GetUserData();
            var userAuth = GetUserAuth();
            fixture.dbAuth.Add(userData);
            fixture.dbAuth.Add(userAuth);
            await fixture.dbAuth.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(userData.Login);

            // assert
            var actualUserData = await fixture.dbAuth.UserData.FirstOrDefaultAsync(i => i.Login == userData.Login);
            var actualUserAuth = await fixture.dbAuth.UserAuth.FirstOrDefaultAsync(i => i.Login == userData.Login);
            Assert.Null(actualUserData);
            Assert.Null(actualUserAuth);
        }

        [Fact]
        public async void UpdateUserData_AtInitializedDbTable_UpdatedUserDataEqualExpectedUserData()
        {
            // arrange
            var userData = GetUserData();
            fixture.dbAuth.Add(userData);
            await fixture.dbAuth.SaveChangesAsync();

            var expected = new UserData
            {
                Login = "admin",
                RoleName = "bestAdmin"
            };

            // act
            await logic.UpdateDataModelAsync(expected);

            // assert
            var actual = await fixture.dbAuth.UserData.FirstOrDefaultAsync(i => i.Login == userData.Login);
            Assert.Equal(expected.Login, actual.Login);
            Assert.Equal(expected.RoleName, actual.RoleName);
        }

        [Fact]
        public async void UpdateUserAuth_AtInitializedDbTable_UpdatedUserAuthEqualExpectedUserAuth()
        {
            // arrange
            var userAuth = GetUserAuth();
            userAuth.PasswordHash = "notHashOfficer";

            fixture.dbAuth.Add(userAuth);
            await fixture.dbAuth.SaveChangesAsync();

            var userAuthUpdateModel = new UserAuthUpdateModel
            {
                Login = "admin",
                Password = "tetriandoh",
                PasswordConfirm = "tetriandoh"
            };

            var expected = new UserAuth
            {
                Login = "admin",
                PasswordHash = "hash"
            };

            // act
            await logic.UpdateDataModelAsync(userAuthUpdateModel);

            // assert
            var actual = await fixture.dbAuth.UserAuth.FirstOrDefaultAsync(i => i.Login == userAuth.Login);
            Assert.Equal(expected.Login, actual.Login);
            Assert.Equal(expected.PasswordHash, actual.PasswordHash);
        }

        [Fact]
        public async void GetUserDataList_FromInitializedDbTables_LogicUserDataListEqualExpectedUserDataList()
        {
            // arrange
            var usersData = GetUsersData();

            fixture.dbAuth.UserData.AddRange(usersData);
            await fixture.dbAuth.SaveChangesAsync();


            var expected = new UserData[]
            {
                new UserData
                {
                    Login = "admin",
                    RoleName = "admin"
                },
                new UserData
                {
                    Login = "technologist",
                    RoleName = "technologist"
                },
                new UserData
                {
                    Login = "seniorTechnologist",
                    RoleName = "seniorTechnologist"
                },
            };

            // act
            var actual = (await logic.GetAllDataModelAsync()).ToList();

            // assert
            foreach (var expectedItem in expected)
            {
                Assert.Contains(actual, actualItem =>
                    expectedItem.Login == actualItem.Login &&
                    expectedItem.RoleName == actualItem.RoleName);
            }
        }

        [Fact]
        public async void GetUserData_FromInitializedDbTables_LogicUserDataEqualExpectedUserData()
        {
            // arrange
            var userData = GetUserData();

            fixture.dbAuth.Add(userData);
            await fixture.dbAuth.SaveChangesAsync();


            var expected = new UserData
            {
                Login = "admin",
                RoleName = "admin"
            };
            
            // act
            var actual = await logic.GetSingleDataModelAsync(expected.Login);

            // assert
            Assert.Equal(expected.Login, actual.Login);
            Assert.Equal(expected.RoleName, actual.RoleName);
        }

        [Fact]
        public async void GetUserToUpdate_FromInitializedDbTable_LogicUserToUpdateEqualExpectedUserToUpdate()
        {
            // arrange
            var userData = GetUserData();

            fixture.dbAuth.Add(userData);
            await fixture.dbAuth.SaveChangesAsync();


            var expected = new UserData
            {
                Login = "admin",
                RoleName = "admin"
            };

            // act
            var actual = await logic.GetSingleUpdateModelAsync(expected.Login);

            // assert
            Assert.Equal(expected.Login, actual.Login);
            Assert.Equal(expected.RoleName, actual.RoleName);
        }
    }
}
