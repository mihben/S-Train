using STrain.CQS.MVC.GenericRequestHandling;
using STrain.Sample.Backend.Services;
using STrain.Sample.Backend.Wireup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseLightInject();

builder.Services.AddMvc();

builder.AddCQS(CQSWireUp.Build);

builder.Services.AddTransient<ISampleService, SampleService>();

builder.Services.AddControllers(options => options.ModelBinderProviders.Insert(0, new RequestModelBinderProvider()));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapControllers();

app.Run();

namespace STrain.Sample.Backend
{
    public class Program { }
}