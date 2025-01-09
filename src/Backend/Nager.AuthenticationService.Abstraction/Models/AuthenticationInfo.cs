using System;

namespace Nager.AuthenticationService.Abstraction.Models
{
    public class AuthenticationInfo
    {
        public DateTime LastValid { get; set; }

        public DateTime LastInvalid { get; set; }

        public int InvalidCount { get; set; }
    }
}
