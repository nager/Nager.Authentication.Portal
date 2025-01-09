using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.AuthenticationService.Abstraction;
using Nager.AuthenticationService.Abstraction.Models;
using Nager.AuthenticationService.Abstraction.Services;
using Nager.AuthenticationService.UnitTest.Helpers;
using Nager.AuthenticationService.WebApi.Helpers;
using Nager.AuthenticationService.WebApi.Services;
using System.Threading.Tasks;

namespace Nager.AuthenticationService.UnitTest
{
    [TestClass]
    public class UserServiceTest
    {
        [TestMethod]
        public async Task ValidateCredentialsAsync_10Retrys_Block()
        {
            var userManagementLoggerMock = LoggerHelper.GetLogger<UserManagementService>();
            var userAuthenticationLoggerMock = LoggerHelper.GetLogger<UserAuthenticationService>();

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var userInfos = new UserInfoWithPassword[]
            {
                new UserInfoWithPassword
                {
                    EmailAddress = "admin@domain.com",
                    Password = "secret",
                    Roles = new [] { "Administrator" }
                }
            };

            IUserRepository userRepository = new InMemoryUserRepository();

            IUserManagementService userManagementService = new UserManagementService(userManagementLoggerMock.Object, userRepository);
            IUserAuthenticationService userService = new UserAuthenticationService(userAuthenticationLoggerMock.Object, userRepository, memoryCache);

            await InitialUserHelper.CreateUsersAsync(userInfos, userManagementService);

            var authenticationRequest = new AuthenticationRequest
            {
                IpAddress = "1.2.3.4",
                EmailAddress = "admin@domain.com",
                Password = "invalidPassword"
            };

            for (var i = 0; i <= 10; i++)
            {
                var authenticationResult = await userService.ValidateCredentialsAsync(authenticationRequest);
                Assert.AreEqual(AuthenticationStatus.Invalid, authenticationResult.Status, $"Retry: {i}");
            }

            var authenticationResult1 = await userService.ValidateCredentialsAsync(authenticationRequest);
            Assert.AreEqual(AuthenticationStatus.TemporaryBlocked, authenticationResult1.Status);
        }

        [TestMethod]
        public async Task ValidateCredentialsAsync_UpperCaseUsername_Allow()
        {
            var userManagementLoggerMock = LoggerHelper.GetLogger<UserManagementService>();
            var userAuthenticationLoggerMock = LoggerHelper.GetLogger<UserAuthenticationService>();

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var userInfos = new UserInfoWithPassword[]
            {
                new UserInfoWithPassword
                {
                    EmailAddress = "admin@domain.com",
                    Password = "secretPassword",
                    Roles = new [] { "Administrator" }
                }
            };

            
            IUserRepository userRepository = new InMemoryUserRepository();

            IUserManagementService userManagementService = new UserManagementService(userManagementLoggerMock.Object, userRepository);
            IUserAuthenticationService userService = new UserAuthenticationService(userAuthenticationLoggerMock.Object, userRepository, memoryCache);

            await InitialUserHelper.CreateUsersAsync(userInfos, userManagementService);

            var authenticationRequest = new AuthenticationRequest
            {
                IpAddress = "1.2.3.4",
                EmailAddress = "admin@domain.com",
                Password = "secretPassword"
            };

            var authenticationResult = await userService.ValidateCredentialsAsync(authenticationRequest);
            Assert.AreEqual(AuthenticationStatus.Valid, authenticationResult.Status);
        }
    }
}
