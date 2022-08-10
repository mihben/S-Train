using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace STrain.CQS.MVC.GenericRequestHandling
{
    public class RequestModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType?.IsAssignableTo(typeof(Command)) ?? false) return new BinderTypeModelBinder(typeof(CommandModelBinder));

            return null;
        }
    }
}
