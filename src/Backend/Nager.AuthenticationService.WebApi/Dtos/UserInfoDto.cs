namespace Nager.AuthenticationService.WebApi.Dtos
{
    /// <summary>
    /// User Info Dto
    /// </summary>
    public class UserInfoDto
    {
        /// <summary>
        /// Unique Id
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// EmailAddress
        /// </summary>
        public required string EmailAddress { get; set; }

        /// <summary>
        /// Roles
        /// </summary>
        public string[] Roles { get; set; } = [];

        /// <summary>
        /// Firstname
        /// </summary>
        public string? Firstname { get; set; }

        /// <summary>
        /// Lastname
        /// </summary>
        public string? Lastname { get; set; }

        /// <summary>
        /// Last Failed Validation Timestamp
        /// </summary>
        public DateTime? LastFailedValidationTimestamp { get; set; }

        /// <summary>
        /// Last Successful Validation Timestamp
        /// </summary>
        public DateTime? LastSuccessfulValidationTimestamp { get; set; }

        /// <summary>
        /// Is Mfa Active
        /// </summary>
        public bool MfaActive { get; set; }
    }
}
