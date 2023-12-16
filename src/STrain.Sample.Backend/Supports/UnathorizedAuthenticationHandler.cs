using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace STrain.Sample.Backend.Supports
{
    public class UnauthorizedAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public UnauthorizedAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync() => Task.FromResult(AuthenticateResult.Fail("Unathorized"));
    }
}
