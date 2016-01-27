namespace Scopes
{
    public class Handler
    {
        public Handler(IPrincipalSlim principal)
        {
            Principal = principal;
        }

        public IPrincipalSlim Principal { get; private set; }
    }
}
