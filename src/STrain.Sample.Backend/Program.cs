using Microsoft.AspNetCore.Authentication;
using Serilog;
using STrain.CQS.NetCore;
using STrain.Sample.Backend.Services;
using STrain.Sample.Backend.Supports;
using STrain.Sample.Backend.Wireup;
using STrain.Tracing.NetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseLightInject();

builder.Logging.AddSerilog(new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger());

builder.Services.AddMvc()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNameCaseInsensitive = true);

builder.Services.AddAuthorization(options => options.AddPolicy("Forbidden", policy => policy.RequireUserName("Admin")));
builder.Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, AuthorizedAuthenticationHandler>(authenticationScheme: "Authorized", null, null)
    .AddScheme<AuthenticationSchemeOptions, UnauthorizedAuthenticationHandler>("Unathorized", null, null)
    .AddScheme<AuthenticationSchemeOptions, ForbiddenAuthenticationHandler>("Forbidden", null, null);

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient();
builder.AddCQS(CQSWireUp.Build);

builder.Services.AddTransient<ISampleService, SampleService>();

var app = builder.Build();

app.UseDefaultExceptionHandler();

app.UseTracing();

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapGenericRequestController();

app.MapControllers();

app.Run();

#pragma warning disable CA1050
public partial class Program { }
#pragma warning restore CA1050