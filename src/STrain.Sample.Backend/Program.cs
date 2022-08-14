using STrain.Sample.Backend.Services;
using STrain.Sample.Backend.Wireup;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddLoki();
//builder.Logging.AddLoki(options =>
//{
//    options.Formatter = LokiLoggingProvider.Options.Formatter.Json;
//    options.DynamicLabels = new LokiLoggingProvider.Options.DynamicLabelOptions
//    {
//        IncludeCategory = true,
//        IncludeEventId = true,
//        IncludeException = true,
//        IncludeLogLevel = true
//    };
//    options.Client = LokiLoggingProvider.Options.PushClient.Http;
//    options.Http = new LokiLoggingProvider.Options.HttpOptions
//    {
//        Address = "https://loki.mihben.work"
//    };
//    options.StaticLabels = new LokiLoggingProvider.Options.StaticLabelOptions
//    {
//        AdditionalStaticLabels = new Dictionary<string, object?>
//        {
//            ["Application"] = "STrain.Sample.Backend",
//            ["Environment"] = "Develop"
//        }
//    };
//});

builder.Host.UseLightInject();

builder.Services.AddMvc();

builder.AddCQS(CQSWireUp.Build);

builder.Services.AddTransient<ISampleService, SampleService>();

var app = builder.Build();

app.MapGenericRequestController();

app.MapControllers();

app.Run();