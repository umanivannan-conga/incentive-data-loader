using System.Text.Json.Serialization;

namespace IncentiveDataLoader.Models
{
	public class RecordAttributes
	{
		[JsonPropertyName("type")]
		public string Type { get; set; }
		[JsonPropertyName("referenceId")]
		public string ReferenceId { get; set; }
	}
}