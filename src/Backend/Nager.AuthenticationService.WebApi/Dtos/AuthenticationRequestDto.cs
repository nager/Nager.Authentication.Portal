using System.ComponentModel.DataAnnotations;

namespace Nager.AuthenticationService.WebApi.Dtos
{
    public class AuthenticationRequestDto
    {
        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
