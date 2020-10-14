using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IncentiveDataLoader.RecordTypes
{
	public class ImportFile<T> where T : Record
	{
		[JsonPropertyName("records")]
		public List<T> Records { get; set; }
	}
}