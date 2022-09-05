using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace STrain.Sample.Backend.Supports
{
    public class UnauthorizedAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public UnauthorizedAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync() => Task.FromResult(AuthenticateResult.Fail("Unathorized"));
    }
}
