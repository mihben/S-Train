using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using STrain;
using STrain.CQS.Blazor.Lightinject;
using STrain.Sample.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient();
builder.UseLightinject();
builder.UseRequestRouter(_ => "backend")
    .AddHttpSender("backend", (options, _) => { options.BaseAddress = new Uri("http://localhost:5100/"); options.Path = "api"; }, builder => builder.UseAttributePathProvider().UseAttributiveMethodProvider().UseAttributeParameterProvider().UseDefaultResponseReader().UseRequestErrorHandler());

await builder.Build().RunAsync();
