using Microsoft.AspNetCore.Authentication;
using Serilog;
using STrain.Sample.Backend.Controllers;
using STrain.Sample.Backend.Services;
using STrain.Sample.Backend.Supports;
using STrain.Sample.Backend.Wireup;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseLightInject();

builder.Logging.AddSerilog(new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger());

builder.Services.AddMvc()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNameCaseInsensitive = true)
    .AddControllersAsServices()
    .AddApplicationPart(typeof(ExternalController).Assembly);
builder.Services.AddControllers();

builder.Services.AddAuthorization(options => options.AddPolicy("Forbidden", policy => policy.RequireUserName("Admin")));
builder.Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, UnathorizedAuthenticationHandler>("Unathorized", null, null)
    .AddScheme<AuthenticationSchemeOptions, ForbiddenAuthenticationHandler>("Forbidden", null, null);

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

public partial class Program { }