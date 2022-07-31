using STrain.CQS.NetCore.Builders;
using STrain.Sample.Backend.Performers;
using STrain.Sample.Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseLightInject();

builder.Services.AddMvc();

builder.AddCQS(builder => builder.AddPerformersFrom(typeof(SampleCommandPerformer).Assembly));

builder.Services.AddTransient<ISampleService, SampleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapControllers();

app.Run();