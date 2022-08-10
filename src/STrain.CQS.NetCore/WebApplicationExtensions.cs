namespace Microsoft.AspNetCore.Builder
{
    public static class WebApplicationExtensions
    {
        public static void MapGenericRequestController(this WebApplication application)
        {
            application.MapControllerRoute("Generic.Command", "api", defaults: new { controller = "GenericRequest", action = "Post" });
            application.MapControllerRoute("Generic.Query", "api", defaults: new { controller = "GenericRequest", action = "Get" });
        }
    }
}
