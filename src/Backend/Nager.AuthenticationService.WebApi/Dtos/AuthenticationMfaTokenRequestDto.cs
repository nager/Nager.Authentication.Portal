using System.ComponentModel.DataAnnotations;

namespace Nager.AuthenticationService.WebApi.Dtos
{
    public class AuthenticationMfaTokenRequestDto
    {
        [Required]
        public string MfaIdentifier { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
