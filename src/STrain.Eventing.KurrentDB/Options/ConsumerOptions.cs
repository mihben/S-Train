using System.ComponentModel.DataAnnotations;

namespace STrain.Eventing.KurrentDB.Options
{
	public class ConsumerOptions
	{
		[Required]
		public required string Stream { get; set; }
	}
}
