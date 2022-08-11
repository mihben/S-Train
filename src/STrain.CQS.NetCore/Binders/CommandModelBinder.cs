using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace STrain.CQS.NetCore.Binders
{
    public class CommandModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            throw new NotImplementedException();
        }
    }

    public class CommandModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.BindingInfo.BinderType?.BaseType?.Equals(typeof(Command)) ?? false) return new CommandModelBinder();

            return null;
        }
    }
}
