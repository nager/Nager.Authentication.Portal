using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Nager.AuthenticationService.WebApi.Dtos;

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
        /// Get Cache Snapshot
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "administrator")]
        [Route("Cache")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult<CacheItemDto> GetCacheSnapshot(
            CancellationToken cancellationToken = default)
        {
            if (this._memoryCache is MemoryCache memoryCache)
            {
                var cacheItems = new List<CacheItemDto>();

                foreach (var key in memoryCache.Keys)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    if (this._memoryCache.TryGetValue<int>(key, out var value))
                    {
                        cacheItems.Add(new CacheItemDto { Key = $"{key}", Value = $"{value}" });
                    }
                }

                return StatusCode(StatusCodes.Status200OK, cacheItems);
            }

            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}
