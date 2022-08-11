using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using STrain.CQS.Api;

namespace STrain.CQS.MVC.GenericRequestHandling
{
    public class RequestModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType.IsAssignableTo(typeof(ICommand)) ||
                context.Metadata.ModelType.IsAssignableTo(typeof(IQuery)))
            {
                return new BinderTypeModelBinder(typeof(RequestModelBinder));
            }

            return null;
        }
    }
}
