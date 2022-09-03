using LightInject;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using STrain;
using STrain.CQS.MVC.Authorization;
using STrain.CQS.MVC.Receiving;
using STrain.CQS.Performers;
using STrain.CQS.Receivers;
using STrain.Sample.Backend.Services;
using STrain.Sample.Backend.Wireup;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseLightInject();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddMvc()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNameCaseInsensitive = true)
    .AddControllersAsServices();
builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient();
builder.AddCQS(CQSWireUp.Build);

builder.Services.AddTransient<ISampleService, SampleService>();

var app = builder.Build();

app.UseDefaultExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapGenericRequestController();

app.MapControllers();

app.Run();