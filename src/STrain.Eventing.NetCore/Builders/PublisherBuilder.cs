using Microsoft.AspNetCore.Builder;

namespace STrain.Eventing.NetCore.Builders
{
	public class PublisherBuilder
	{
		public WebApplicationBuilder Builder { get; }

		public PublisherBuilder(WebApplicationBuilder builder)
		{
			Builder = builder;
		}
	}
}
