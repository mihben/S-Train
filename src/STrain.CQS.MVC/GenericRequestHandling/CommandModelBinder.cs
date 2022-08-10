using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace STrain.CQS.MVC.GenericRequestHandling
{
    public class CommandModelBinder : IModelBinder
    {
        private readonly ILogger<CommandModelBinder> _logger;

        public CommandModelBinder(ILogger<CommandModelBinder> logger)
        {
            _logger = logger;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var value = bindingContext.HttpContext.Request.Headers.GetRequestType();
            var requestType = value is null ? bindingContext.ModelType : Type.GetType(value);
            if (requestType is null) throw new InvalidOperationException($"{value} type is unknown");

            bindingContext.Result = ModelBindingResult.Success(await bindingContext.HttpContext.Request.ReadAsJsonAsync(requestType, bindingContext.HttpContext.RequestAborted));
        }
    }
}
