using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using STrain.CQS.Api;

namespace STrain.CQS.MVC.GenericRequestHandling
{
    public class RequestModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType.IsAssignableTo(typeof(ICommand))) return new BinderTypeModelBinder(typeof(CommandModelBinder));
            return null;
        }
    }
}
