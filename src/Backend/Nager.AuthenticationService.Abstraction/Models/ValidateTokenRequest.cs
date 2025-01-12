namespace Nager.AuthenticationService.Abstraction.Models
{
    /// <summary>
    /// Validate Token Request
    /// </summary>
    public class ValidateTokenRequest
    {
        /// <summary>
        /// Mfa Identifier from Authenticate request
        /// </summary>
        public required string MfaIdentifier { get; set; }

        /// <summary>
        /// Mfa Token
        /// </summary>
        public required string Token { get; set; }

        /// <summary>
        /// IpAddress
        /// </summary>
        public string? IpAddress { get; set; }
    }
}
