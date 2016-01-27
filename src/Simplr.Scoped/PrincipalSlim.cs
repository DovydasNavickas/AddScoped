using System.Collections.Generic;

namespace Scopes
{
    public class PrincipalSlim : IPrincipalSlim
    {
        public string UserId { get; set; }
        public IDictionary<string, string> Claims { get; set; }
    }
}
