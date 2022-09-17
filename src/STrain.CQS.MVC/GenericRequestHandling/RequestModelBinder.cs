using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace STrain.CQS.MVC.GenericRequestHandling
{
    public class RequestModelBinder : IModelBinder
    {
        private readonly ILogger<RequestModelBinder> _logger;

        public RequestModelBinder(ILogger<RequestModelBinder> logger)
        {
            _logger = logger;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var value = bindingContext.HttpContext.Request.Headers.GetRequestType();
            var requestType = value is null ? bindingContext.ModelType : Type.GetType(value);
            if (requestType is null) throw new InvalidOperationException($"Unkown {requestType} type");

            _logger.LogDebug("Attempting to bind parameter {ParameterName} of type {ModelType}", bindingContext.ModelMetadata?.ParameterName, requestType);
            if (bindingContext.HttpContext.Request.ContentLength is null) bindingContext.Result = ModelBindingResult.Success(Activator.CreateInstance(requestType));
            else bindingContext.Result = ModelBindingResult.Success(await bindingContext.HttpContext.Request.ReadAsJsonAsync(requestType, bindingContext.HttpContext.RequestAborted));
            _logger.LogDebug("Done attempting to bind parameter {ParameterName} of type {ModelType}", bindingContext.ModelMetadata?.ParameterName, requestType);
        }
    }
}
