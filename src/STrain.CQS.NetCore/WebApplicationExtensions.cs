using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using STrain.CQS.MVC.Options;
using STrain.CQS.NetCore.ErrorHandling;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebApplicationExtensions
    {
        public static void MapGenericRequestController(this WebApplication application)
        {
            var options = application.Services.GetRequiredService<IOptions<GenericRequestHandlerOptions>>();

            application.MapControllerRoute("Generic.Command", options.Value.Path, defaults: new { controller = "GenericRequest", action = "Post" });
            application.MapControllerRoute("Generic.Query", options.Value.Path, defaults: new { controller = "GenericRequest", action = "Get" });
        }

        public static void UseDefaultExceptionHandler(this WebApplication application)
        {
            application.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
