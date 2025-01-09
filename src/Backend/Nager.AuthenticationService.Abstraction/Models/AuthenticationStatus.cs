namespace Nager.AuthenticationService.Abstraction.Models
{
    public enum AuthenticationStatus
    {
        Invalid,
        Valid,
        MfaCodeRequired,
        TemporaryBlocked
    }
}
