using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using STrain;
using STrain.Sample.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.UseLightinject();
builder.UseRequestRouter(_ => "backend")
    .AddHttpSender("backend");

await builder.Build().RunAsync();
