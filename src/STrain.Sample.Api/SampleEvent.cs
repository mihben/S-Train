namespace STrain.Sample.Api
{
	public record SampleEvent : Event
	{
		public required string Value { get; init; }
	}
}
