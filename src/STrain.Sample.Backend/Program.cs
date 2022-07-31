using STrain.CQS.NetCore.Builders;
using STrain.Sample.Backend.Performers;
using STrain.Sample.Backend.Services;
using STrain.Sample.Backend.Wireup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseLightInject();

builder.Services.AddMvc();

builder.AddCQS(CQSWireUp.Build);

builder.Services.AddTransient<ISampleService, SampleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapControllers();

app.Run();