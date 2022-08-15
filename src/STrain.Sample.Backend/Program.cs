using STrain.Sample.Backend.Services;
using STrain.Sample.Backend.Wireup;
using LokiLoggingProvider.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseLightInject();

builder.Services.AddMvc();

builder.AddCQS(CQSWireUp.Build);

builder.Services.AddTransient<ISampleService, SampleService>();

var app = builder.Build();

app.MapGenericRequestController();

app.MapControllers();

app.Run();