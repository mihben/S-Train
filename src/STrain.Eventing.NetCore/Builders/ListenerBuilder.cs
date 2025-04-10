using Microsoft.AspNetCore.Builder;

namespace STrain.Eventing.NetCore.Builders
{
	public class ListenerBuilder
	{
		public WebApplicationBuilder Builder { get; }

		public ListenerBuilder(WebApplicationBuilder builder)
		{
			Builder = builder;
		}
	}
}
