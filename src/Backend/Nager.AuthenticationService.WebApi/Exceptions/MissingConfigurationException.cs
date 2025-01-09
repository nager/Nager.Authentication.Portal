namespace Nager.AuthenticationService.WebApi.Models
{
    public class MissingConfigurationException : Exception
    {
        private readonly string _message;

        public MissingConfigurationException(string message)
        {
            this._message = message;
        }
    }
}
