using System.Collections.Generic;

namespace Scopes
{
    public interface IPrincipalSlim
    {
        IDictionary<string, string> Claims { get; set; }
        string UserId { get; set; }
    }
}