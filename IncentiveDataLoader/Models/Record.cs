using System.Text.Json.Serialization;

namespace IncentiveDataLoader.Models
{
	public class Record
	{
		[JsonPropertyName("attributes")]
		public RecordAttributes Attributes { get; set; }
	}
}