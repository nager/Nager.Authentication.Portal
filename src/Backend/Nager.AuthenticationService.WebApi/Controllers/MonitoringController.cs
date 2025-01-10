using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Nager.AuthenticationService.WebApi.Controllers
{
    /// <summary>
    /// Monitoring Controller
    /// </summary>
    [ApiController]
    [ApiExplorerSettings(GroupName = "monitoring")]
    [Authorize]
    [Route("auth/api/v1/[controller]")]
    public class MonitoringController : ControllerBase
    {
        private readonly ILogger<MonitoringController> _logger;
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Monitoring Controller
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="memoryCache"></param>
        public MonitoringController(
            ILogger<MonitoringController> logger,
            IMemoryCache memoryCache)
        {
            this._logger = logger;
            this._memoryCache = memoryCache;
        }

        /// <summary>
        /// Get Cache Keys
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "administrator")]
        [Route("Cache")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> GetAllCacheKeys(
            CancellationToken cancellationToken = default)
        {
            if (this._memoryCache is MemoryCache memoryCache)
            {
                var statistic = new List<string>();

                foreach (var key in memoryCache.Keys)
                {
                    this._memoryCache.TryGetValue<int>(key, out int value);

                    statistic.Add($"{key}:{value}");
                }

                return StatusCode(StatusCodes.Status200OK, statistic);
            }

            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}
