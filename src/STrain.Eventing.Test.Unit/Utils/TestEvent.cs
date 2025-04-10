namespace STrain.Eventing.Test.Unit.Utils
{
	public record TestEvent1 : Event
	{
		public string Value { get; init; } = null!;
	}

	public record TestEvent2 : Event
	{
		public int Number { get; init; }
	}
}
