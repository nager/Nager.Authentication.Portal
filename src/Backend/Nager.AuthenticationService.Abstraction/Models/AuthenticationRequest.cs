namespace Nager.AuthenticationService.Abstraction.Models
{
    /// <summary>
    /// Authentication Request
    /// </summary>
    public class AuthenticationRequest
    {
        /// <summary>
        /// Email Address
        /// </summary>
        public required string EmailAddress { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public required string Password { get; set; }

        /// <summary>
        /// IpAddress
        /// </summary>
        public string? IpAddress { get; set; }
    }
}
