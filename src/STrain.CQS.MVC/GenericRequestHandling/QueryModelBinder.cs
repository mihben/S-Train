using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Reflection;

namespace STrain.CQS.MVC.GenericRequestHandling
{
    public class QueryModelBinder : IModelBinder
    {
        private readonly ILogger<QueryModelBinder> _logger;

        public QueryModelBinder(ILogger<QueryModelBinder> logger)
        {
            _logger = logger;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var requestTypeValue = bindingContext.HttpContext.Request.Headers.GetRequestType();
            var requestType = requestTypeValue is null ? bindingContext.ModelType : Type.GetType(requestTypeValue);
            if (requestType is null) throw new InvalidOperationException($"Unkown {requestType} type");

            _logger.LogDebug("Attempting to bind parameter {ParameterName} of type {ModelType}", bindingContext.ModelMetadata?.ParameterName, requestType);
            var constructor = requestType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, requestType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => p.PropertyType).ToArray());
            if (constructor is null) throw new InvalidOperationException($"Matching constructor not found for {requestType}");

            var attributes = new List<object>();
            foreach (var parameter in constructor.GetParameters())
            {
                var result = bindingContext.ValueProvider.GetValue(parameter.Name);
                attributes.Add(result.ParseTo(parameter.ParameterType));
            }
            bindingContext.Result = ModelBindingResult.Success(Activator.CreateInstance(requestType, attributes.ToArray()));
            _logger.LogDebug("Done attempting to bind parameter {ParameterName} of type {ModelType}", bindingContext.ModelMetadata?.ParameterName, requestType);
            return Task.CompletedTask;
        }
    }

    internal static class QueryModelBinderExtensions
    {
        public static dynamic ParseTo(this ValueProviderResult values, Type target)
        {
            if (target.Equals(typeof(string))) return values.FirstValue;
            if (target.GetInterface(nameof(IEnumerable)) != null)
            {
                var result = new List<int>();
                foreach (var value in values)
                {
                    result.Add(value.ParseTo(target.GetTypeInfo().GenericTypeArguments.First()));
                }
                return result;
            }

            return values.FirstValue.ParseTo(target);
        }

        public static dynamic ParseTo(this string value, Type target)
        {
            if (target.Equals(typeof(int))) return int.Parse(value);
            else if (target.Equals(typeof(Guid))) return Guid.Parse(value);
            else if (target.Equals(typeof(double))) return double.Parse(value);
            else if (target.Equals(typeof(long))) return long.Parse(value);
            else if (target.Equals(typeof(string))) return value;
            else if (target.Equals(typeof(DateTime))) return DateTime.Parse(value);
            else if (target.Equals(typeof(bool))) return bool.Parse(value);
            else if (target.Equals(typeof(char))) return char.Parse(value);
            else throw new NotSupportedException($"{target} is unsupported.");
        }
    }
}
