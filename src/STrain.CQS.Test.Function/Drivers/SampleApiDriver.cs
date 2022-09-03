using LightInject;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using STrain.CQS.NetCore.RequestSending;
using STrain.CQS.NetCore.RequestSending.Providers;
using STrain.CQS.Senders;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Function.Drivers
{
    internal static class SampleApiDriver
    {
        public static WebApplicationFactory<Program> Initialize(this WebApplicationFactory<Program> driver, ITestOutputHelper outputHelper)
        {
            return driver
                .WithWebHostBuilder(builder => builder.ConfigureLogging(builder => builder.AddXUnit(outputHelper)));
        }

        public static WebApplicationFactory<Program> MockHttpSender(this WebApplicationFactory<Program> driver, string key, HttpMessageHandler messageHandler, string path, string baseAddress)
        {
            return driver.WithWebHostBuilder(builder =>
             {
                 builder.ConfigureAppConfiguration((_, builder) => builder.AddInMemoryCollection(new Dictionary<string, string>
                 {
                     ["Senders:Internal:Path"] = path
                 }));
                 builder.ConfigureTestContainer<IServiceContainer>(registry =>
                 {
                     var httpClient = new HttpClient(messageHandler)
                     {
                         BaseAddress = new Uri(baseAddress)
                     };
                     registry.Override(registration => registration.ServiceName.Equals(key), (factory, registration) =>
                     {
                         registration.Lifetime = new PerContainerLifetime();

                         using (var scope = factory.BeginScope())
                         {
                             var pathProvider = scope.GetInstance<IPathProvider>(key);
                             var methodProvider = scope.GetInstance<IMethodProvider>(key);
                             var parameterProviders = new List<IParameterProvider>
                             {
                                 scope.GetInstance<IParameterProvider>($"{key}.header"),
                                 scope.GetInstance<IParameterProvider>($"{key}.query"),
                                 scope.GetInstance<IParameterProvider>($"{key}.body")
                             };
                             var responseReaderProvider = scope.GetInstance<IResponseReaderProvider>(key);
                             var requestErrorHandler = scope.GetInstance<IRequestErrorHandler>(key);


                             registration.Value = new HttpRequestSender(httpClient, scope.GetInstance<IServiceProvider>(), pathProvider, methodProvider, parameterProviders, responseReaderProvider, requestErrorHandler, scope.GetInstance<ILogger<HttpRequestSender>>());
                         }

                         return registration;
                     });
                 });
             });
        }

        public static async Task<T?> SendRequestAsync<TRequest, T>(this WebApplicationFactory<Program> driver, TRequest request, TimeSpan timeout)
            where TRequest : IRequest
        {
            var sender = driver.Services.GetRequiredService<IRequestSender>();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            return await sender.SendAsync<TRequest, T>(request, cancellationTokenSource.Token);
        }
        public static async Task<T?> SendQueryAsync<TQuery, T>(this WebApplicationFactory<Program> driver, TQuery query, TimeSpan timeout)
            where TQuery : Query<T>
        {
            var sender = driver.Services.GetRequiredService<IRequestSender>();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            return await sender.GetAsync<TQuery, T>(query, cancellationTokenSource.Token);
        }

        public static async Task SendCommandAsync<TCommand>(this WebApplicationFactory<Program> driver, TCommand command, TimeSpan timeout)
            where TCommand : Command
        {
            var sender = driver.Services.GetRequiredService<IRequestSender>();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            await sender.SendAsync(command, cancellationTokenSource.Token);
        }

        public static async Task<HttpResponseMessage> ReceiveCommandAsync<TCommand>(this WebApplicationFactory<Program> driver, TCommand command, TimeSpan timeout)
            where TCommand : Command
        {
            var client = driver.CreateClient();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            return await client.PostAsync("api", PrepareContent(command), cancellationTokenSource.Token);
        }

        public static async Task<HttpResponseMessage> ReceiveQueryAsync<TQuery, T>(this WebApplicationFactory<Program> driver, TQuery query, TimeSpan timeout)
            where TQuery : Query<T>
        {
            var client = driver.CreateClient();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            var message = new HttpRequestMessage(HttpMethod.Get, "api")
            {
                Content = PrepareContent(query)
            };
            return await client.SendAsync(message, cancellationTokenSource.Token);
        }

        private static HttpContent PrepareContent<TRequest>(TRequest request)
            where TRequest : IRequest
        {
            var content = JsonContent.Create(request);
            content.Headers.Add("Request-Type", $"{request.GetType().FullName}, {request.GetType().Assembly.GetName().FullName}");

            return content;
        }

        public static async Task<HttpResponseMessage> SendAsync(this WebApplicationFactory<Program> driver, string endpoint, TimeSpan timeout)
        {
            var client = driver.CreateClient();
            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            return await client.GetAsync(endpoint, cancellationTokenSource.Token);
        }

        public static AuthenticationBuilder Forbidden(this AuthenticationBuilder builder)
        {
            return builder.AddScheme<AuthenticationSchemeOptions, ForbiddenAuthenticationHandler>("Forbidden", _ => { });
        }

        public static AuthenticationBuilder Unathorized(this AuthenticationBuilder builder)
        {
            return builder.AddScheme<AuthenticationSchemeOptions, UnathorizedAuthenticationHandler>("Unathorized", _ => { });
        }

        public static AuthenticationBuilder Authorized(this AuthenticationBuilder builder)
        {
            return builder.AddScheme<AuthenticationSchemeOptions, AuthorizedAuthenticationHandler>("Authorized", _ => { });
        }

        public static WebApplicationFactory<Program> MockAuthentication(this WebApplicationFactory<Program> driver, Action<AuthenticationBuilder> build)
        {
            return driver.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthorization(options => options.AddPolicy("forbidden-policy", policy => policy.RequireUserName("forbidden-user")));
                    services.AddAuthorization(options => options.AddPolicy("Authorized", builder => builder.RequireClaim("Test")));
                    build(services.AddAuthentication("Authorized"));
                });
            });
        }

        internal class AuthorizedAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            public AuthorizedAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
                : base(options, logger, encoder, clock)
            {
            }

            protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                var claims = new[] { new Claim(ClaimTypes.Name, "Test user") };
                var identity = new ClaimsIdentity(claims, "Test");
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, "Test");

                var result = AuthenticateResult.Success(ticket);

                return Task.FromResult(result);
            }
        }

        internal class ForbiddenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            public ForbiddenAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
                : base(options, logger, encoder, clock)
            {
            }

            protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                var claims = new[] { new Claim(ClaimTypes.Name, "Test user") };
                var identity = new ClaimsIdentity(claims, "Test");
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, "Test");

                var result = AuthenticateResult.Success(ticket);

                return Task.FromResult(result);
            }
        }

        internal class UnathorizedAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            public UnathorizedAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
                : base(options, logger, encoder, clock)
            {
            }

            protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                var result = AuthenticateResult.Fail("Test Unathorized request");

                return Task.FromResult(result);
            }
        }
    }
}
