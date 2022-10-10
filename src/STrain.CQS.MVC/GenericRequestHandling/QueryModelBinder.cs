using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;

namespace STrain.CQS.MVC.GenericRequestHandling
{
    public class QueryModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var requestTypeValue = bindingContext.HttpContext.Request.Headers.GetRequestType();
            var requestType = requestTypeValue is null ? bindingContext.ModelType : Type.GetType(requestTypeValue);
            if (requestType is null) throw new InvalidOperationException($"Unkown {requestType} type");

            var constructor = requestType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, requestType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => p.PropertyType).ToArray());
            if (constructor is null) throw new InvalidOperationException($"Matching constructor not found for {requestType}");

            var attributes = new List<object>();
            foreach (var parameter in constructor.GetParameters())
            {
                var result = bindingContext.ValueProvider.GetValue(parameter.Name);
                if (result.Length > 1) attributes.Add(result.Values.AsEnumerable());
                else attributes.Add(result.FirstValue);
            }
            bindingContext.Result = ModelBindingResult.Success(Activator.CreateInstance(requestType, attributes.ToArray()));
            return Task.CompletedTask;
        }
    }
}
