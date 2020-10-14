using System.Text.Json.Serialization;

namespace IncentiveDataLoader.RecordTypes
{
	public class Record
	{
		[JsonPropertyName("attributes")]
		public RecordAttributes Attributes { get; set; }
	}
}