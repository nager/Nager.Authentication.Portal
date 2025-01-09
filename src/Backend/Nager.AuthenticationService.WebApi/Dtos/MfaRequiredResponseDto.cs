namespace Nager.AuthenticationService.WebApi.Dtos
{
    public class MfaRequiredResponseDto
    {
        public string MfaIdentifier { get; set; }
        public string MfaType { get; set; }
    }
}
