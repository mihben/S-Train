using LightInject;
using STrain;
using STrain.CQS.MVC.Authorization;
using STrain.Sample.Backend.Services;
using STrain.Sample.Backend.Wireup;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseLightInject();

builder.Services.AddMvc()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNameCaseInsensitive = true)
    .AddControllersAsServices();
builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<IMvcRequestReceiver, MvcRequestReceiver>();
builder.Host.ConfigureContainer<IServiceRegistry>((_, registry) => registry.Decorate<IMvcRequestReceiver, MvcRequestAuthorizer>());

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