﻿using STrain.CQS.NetCore.Builders;
using STrain.CQS.NetCore.LigtInject;
using STrain.Sample.Backend.Performers;

namespace STrain.Sample.Backend.Wireup
{
    public static class CQSWireUp
    {
        public static void Build(CQSBuilder builder)
        {
            builder.AddPerformerFrom<SampleCommandPerformer>();

            builder.AddGenericRequestHandler("api");

            builder.AddRequestRouter(request =>
                                                            {
                                                                if (request.GetType().Name.StartsWith("External")) return "external";
                                                                else return "internal";
                                                            },
                builder =>
                        {
                            builder.AddHttpSender("external", (options, configuration) => configuration.Bind("Senders:External", options)).UseDefaults();
                            builder.AddHttpSender("internal", (options, configuraion) => configuraion.Bind("Senders:Internal", options)).UseDefaults();
                        });

        }
    }
}
