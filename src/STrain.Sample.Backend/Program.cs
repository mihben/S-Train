using Microsoft.AspNetCore.Authentication.JwtBearer;
using STrain.CQS.NetCore.ErrorHandling;
using STrain.Sample.Backend.Services;
using STrain.Sample.Backend.Wireup;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseLightInject();

builder.Services.AddMvc();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();
builder.Services.AddAuthorization();

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