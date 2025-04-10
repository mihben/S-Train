using Microsoft.Extensions.DependencyInjection;
using STrain.Eventing.NetCore.Builders;

namespace Microsoft.AspNetCore.Builder
{
	public static class ApplicationBuilderExtensions
	{
		public static void AddEventing(this WebApplicationBuilder builder, Action<EventingBuilder> build)
		{
			builder.Services.AddEventing();
			build(new EventingBuilder(builder));
		}
	}
}
