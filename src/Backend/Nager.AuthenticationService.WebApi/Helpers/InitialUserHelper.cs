using Nager.AuthenticationService.Abstraction.Models;
using Nager.AuthenticationService.Abstraction.Services;

namespace Nager.AuthenticationService.WebApi.Helpers
{
    /// <summary>
    /// Initial User Helper
    /// </summary>
    public static class InitialUserHelper
    {
        /// <summary>
        /// Create initial users
        /// </summary>
        /// <param name="items"></param>
        /// <param name="userManagementService"></param>
        /// <returns></returns>
        public static async Task<bool> CreateUsersAsync(
            UserInfoWithPassword[] items,
            IUserManagementService userManagementService)
        {
            var successful = true;

            foreach (var item in items)
            {
                var userInfo = await userManagementService.GetByEmailAddressAsync(item.EmailAddress);
                if (userInfo != null)
                {
                    continue;
                }

                var createRequest = new UserCreateRequest
                {
                    EmailAddress = item.EmailAddress,
                    Password = item.Password,
                    Firstname = item.Firstname,
                    Lastname = item.Lastname,
                    Roles = item.Roles
                };

                if (!await userManagementService.CreateAsync(createRequest))
                {
                    successful = false;
                    //log failure
                }
            }

            return successful;
        }
    }
}
