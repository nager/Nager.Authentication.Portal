namespace Nager.AuthenticationService.WebApi.Models
{
    /// <summary>
    /// Missing Configuration Exception
    /// </summary>
    public class MissingConfigurationException : Exception
    {
        /// <summary>
        /// Missing Configuration Exception
        /// </summary>
        /// <param name="message"></param>
        public MissingConfigurationException(string message) : base(message)
        { }
    }
}
