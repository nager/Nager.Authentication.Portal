namespace Nager.AuthenticationService.WebApi.Extenstions
{
    /// <summary>
    /// HttpContext Extension
    /// </summary>
    public static class HttpContextExtension
    {
        internal static string? GetIpAddress(this HttpContext httpContext)
        {
            if (httpContext.Request != null)
            {
                if (httpContext.Request.Headers.ContainsKey("X-Real-IP"))
                {
                    return httpContext.Request.Headers["X-Real-IP"];
                }

                if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                {
                    return httpContext.Request.Headers["X-Forwarded-For"];
                }
            }

            return httpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}
