using STrain.Sample.Backend.Services;
using STrain.Sample.Backend.Wireup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseLightInject();

builder.Services.AddMvc();

builder.AddCQS(CQSWireUp.Build);

builder.Services.AddTransient<ISampleService, SampleService>();

var app = builder.Build();

app.MapGenericRequestController();

app.MapControllers();

app.Run();

namespace STrain.Sample.Backend
{
    public class Program { }
}