using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Runtime.CompilerServices;

namespace STrain.CQS.MVC.GenericRequestHandling
{
    public class RequestModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType?.IsAssignableTo(typeof(Command)) ?? false) return new BinderTypeModelBinder(typeof(CommandModelBinder));
            if (context.Metadata.ModelType?.IsAssignableToGeneric(typeof(Query<>)) ?? false) return new BinderTypeModelBinder(typeof(QueryModelBinder));

            return null;
        }
    }

    internal static class RequestModelBinderProviderExtensions
    {
        public static bool IsAssignableToGeneric(this Type type, Type target)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableTo(target)) return true;
            if (type.BaseType is null) return false;

            return type.BaseType.IsAssignableToGeneric(target);
        }
    }
}
