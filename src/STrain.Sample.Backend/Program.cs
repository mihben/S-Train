using FluentValidation;
using LightInject;
using STrain.CQS.Dispatchers;
using STrain.CQS.NetCore.ErrorHandling;
using STrain.CQS.NetCore.Validation;
using STrain.CQS.Validations;
using STrain.Sample.Api;
using STrain.Sample.Backend.Services;
using STrain.Sample.Backend.Wireup;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseLightInject();

builder.Services.AddMvc();

builder.Services.AddValidatorsFromAssembly(typeof(SampleCommand).Assembly);
builder.Services.AddTransient<IRequestValidator, FluentRequestValidator>();

builder.Host.ConfigureContainer<IServiceContainer>(container => container.Decorate<ICommandDispatcher, CommandValidator>());

builder.Services.AddHttpClient();
builder.AddCQS(CQSWireUp.Build);

builder.Services.AddTransient<ISampleService, SampleService>();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapGenericRequestController();

app.MapControllers();

app.Run();