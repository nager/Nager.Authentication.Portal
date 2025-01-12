namespace Nager.AuthenticationService.WebApi.Dtos
{
    /// <summary>
    /// Cache Item Dto
    /// </summary>
    public class CacheItemDto
    {
        /// <summary>
        /// Cache Key
        /// </summary>
        public required string Key { get; set; }

        /// <summary>
        /// Cache Data
        /// </summary>
        public string? Value { get; set; }
    }
}
