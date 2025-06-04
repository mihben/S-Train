using KurrentDB.Client;
using STrain.Eventing.Api;
using STrain.Sample.Api;
using STrain.Sample.Backend.Services;
using System.Text.Json;

namespace STrain.Sample.Backend.Performers
{
	public class SampleCommandPerformer : ICommandPerformer<Api.Sample.GenericCommand>
	{
		private readonly ISampleService _sampleService;
		private readonly KurrentDBClient _client;

		public SampleCommandPerformer(ISampleService sampleService, KurrentDBClient client)
		{
			_sampleService = sampleService;
			_client = client;
		}

		public async Task PerformAsync(Api.Sample.GenericCommand command, CancellationToken cancellationToken)
		{
			await _sampleService.DoSampleAsync(command, cancellationToken);
			var @event = new SampleEvent { Value = command.Value };
			await _client.AppendToStreamAsync("sample-events", StreamState.Any, [new EventData(Uuid.NewUuid(), @event.GetEventType(), JsonSerializer.SerializeToUtf8Bytes(@event))], cancellationToken: cancellationToken);
		}
	}
}
