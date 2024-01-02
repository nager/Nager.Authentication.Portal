using Microsoft.EntityFrameworkCore;
using Nager.AuthenticationService.MssqlRepository;

namespace Nager.AuthenticationService.WebApi
{
    /// <summary>
    /// MigrationHelper
    /// </summary>
    public class MigrationHelper
    {
        private readonly ILogger<MigrationHelper> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        /// <summary>
        /// MigrationHelper
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceScopeFactory"></param>
        public MigrationHelper(
            ILogger<MigrationHelper> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            this._logger = logger;
            this._serviceScopeFactory = serviceScopeFactory;
        }

        /// <summary>
        /// Update the database structure
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateDatabaseAsync()
        {
            var retryCount = 60;

            for (var i = 0; i < retryCount; i++)
            {
                using var serviceScope = this._serviceScopeFactory.CreateScope();
                using var context = serviceScope.ServiceProvider.GetService<DatabaseContext>();

                if (context == null)
                {
                    this._logger.LogError($"{nameof(UpdateDatabaseAsync)} - Context is not available");
                    return false;
                }

                try
                {
                    await context.Database.MigrateAsync();
                    return true;
                }
                catch (Exception exception)
                {
                    this._logger.LogError(exception, $"{nameof(UpdateDatabaseAsync)} - Cannot execute database migrate, retry: {i}");
                    await Task.Delay(1000);
                }
            }

            return false;
        }
    }
}
