using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;
using System.Web;

namespace STrain.CQS.MVC.GenericRequestHandling
{
    public class QueryModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var requestTypeValue = bindingContext.HttpContext.Request.Headers.GetRequestType();
            var requestType = requestTypeValue is null ? bindingContext.ModelType : Type.GetType(requestTypeValue);
            if (requestType is null) throw new InvalidOperationException($"Unkown {requestType} type");

            var query = HttpUtility.ParseQueryString(bindingContext.HttpContext.Request.QueryString.Value ?? string.Empty);
            var constructor = requestType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, requestType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => p.PropertyType).ToArray());
            if (constructor is null) throw new InvalidOperationException($"Matching constructor not found for {requestType}");

            var attributes = new List<object>();
            foreach (var parameter in constructor.GetParameters())
            {
                var value = query[parameter.Name] ?? throw new InvalidOperationException($"Value of '{parameter.Name}' parameter was not found");
                attributes.Add(parameter switch
                {
                    { ParameterType: var targetType } when targetType.Equals(typeof(Guid)) => Guid.Parse(value)!,
                    { ParameterType: var targetType } when targetType.Equals(typeof(int)) => int.Parse(value)!,
                    { ParameterType: var targetType } when targetType.Equals(typeof(double)) => double.Parse(value)!,
                    { ParameterType: var targetType } when targetType.Equals(typeof(long)) => long.Parse(value)!,
                    { ParameterType: var targetType } when targetType.Equals(typeof(string)) => value!,
                    { ParameterType: var targetType } when targetType.Equals(typeof(DateTime)) => DateTime.Parse(value)!,
                    { ParameterType: var targetType } when targetType.Equals(typeof(bool)) => bool.Parse(value)!,
                    { ParameterType: var targetType } when targetType.Equals(typeof(char)) => char.Parse(value)!,
                    _ => throw new NotSupportedException($"{parameter.ParameterType} is unsupported.")
                });
            }

            bindingContext.Result = ModelBindingResult.Success(Activator.CreateInstance(requestType, attributes.ToArray()));
            return Task.CompletedTask;
        }
    }
}
