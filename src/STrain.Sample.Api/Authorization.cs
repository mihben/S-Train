namespace STrain.Sample.Api
{
    public static class Authorization
    {
        //[Authorize(AuthenticationSchemes = "Authorized")]
        public record AuthorizedCommand : Command { }

        //[Authorize(AuthenticationSchemes = "Unathorized")]
        public record UnathorizedCommand : Command { }

        //[Authorize(AuthenticationSchemes = "Forbidden", Policy = "Forbidden")]
        public record ForbiddenCommand : Command { }

        //[Authorize(AuthenticationSchemes = "Unathorized")]
        //[AllowAnonymous]
        public record AllowAnonymusCommand : Command { }
    }


}
