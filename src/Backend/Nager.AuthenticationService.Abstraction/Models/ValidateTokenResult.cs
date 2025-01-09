namespace Nager.AuthenticationService.Abstraction.Models
{
    public class ValidateTokenResult
    {
        public bool Success { get; set; }
        public string EmailAddress { get; set; }
    }
}
