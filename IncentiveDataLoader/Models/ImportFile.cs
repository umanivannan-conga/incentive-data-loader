using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IncentiveDataLoader.Models
{
	public class ImportFile<T> where T : Record
	{
		[JsonPropertyName("records")]
		public List<T> Records { get; set; }
	}
}