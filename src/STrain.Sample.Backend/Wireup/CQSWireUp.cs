using STrain.CQS.NetCore.Builders;
using STrain.CQS.NetCore.LigtInject;
using STrain.Sample.Api;
using STrain.Sample.Backend.Performers;

namespace STrain.Sample.Backend.Wireup
{
    public static class CQSWireUp
    {
        public static void Build(CQSBuilder builder)
        {
            builder.AddPerformerFrom<SampleCommandPerformer>();

            builder.AddRequestValidator()
                .UseFluentRequestValidator(builder => builder.RegistrateFrom<Error.ValidatedCommandValidator>());

            builder.AddMvcRequestReceiver()
                .UseAuthorization()
                .UseLogger();

            builder.AddGenericRequestHandler("api");

            builder.AddRequestRouter(request =>
            {
                if (request.GetType().Name.Contains("External")) return "External";
                else return "Generic";
            },
                builder => builder.AddHttpSender("Generic", (options, configuraion) => configuraion.Bind("Senders:Generic", options)));
        }
    }
}
