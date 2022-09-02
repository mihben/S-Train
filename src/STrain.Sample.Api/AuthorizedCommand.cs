using Microsoft.AspNetCore.Authorization;

namespace STrain.Sample.Api
{
    [Authorize(AuthenticationSchemes = "Authorized")]
    public record AuthorizedCommand : Command
    {
    }

    [Authorize(AuthenticationSchemes = "Unathorized")]
    public record UnathorizedCommand : Command
    {

    }

    [Authorize("forbidden-policy")]
    public record ForbiddenCommand : Command
    {
    }

    [Authorize(AuthenticationSchemes = "Unathorized")]
    [AllowAnonymous]
    public record AllowAnonymusCommand : Command
    {

    }
}
