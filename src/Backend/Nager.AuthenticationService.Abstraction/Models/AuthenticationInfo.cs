using System;

namespace Nager.AuthenticationService.Abstraction.Models
{
    public class AuthenticationInfo
    {
        public DateTime LastValid { get; set; }

        public DateTime LastInvalid { get; set; }

        public int InvalidCount { get; set; }

        public override string ToString()
        {
            return $"LastValid:{this.LastValid} LastInvalid:{this.LastInvalid} InvalidCount:{this.InvalidCount}";
        }
    }
}
