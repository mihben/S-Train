using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using STrain.Sample.Backend;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.Support
{
    public class SampleHost : WebApplicationFactory<Program>
    {
        private readonly ITestOutputHelper _outputHelper;

        public SampleHost(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureTestServices(services => services.AddLogging(builder => builder
                                                                                                                    .AddXUnit(_outputHelper)
                                                                                                                    .SetMinimumLevel(LogLevel.Trace)));
        }
    }
}
