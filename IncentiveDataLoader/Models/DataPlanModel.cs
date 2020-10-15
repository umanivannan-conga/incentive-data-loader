using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace IncentiveDataLoader.Models
{
	public class DataPlanModel
	{
		[JsonPropertyName("sobject")]
		public string SObject { get; set; }
		[JsonPropertyName("saveRefs")]
		public bool SaveRefs { get; set; }
		[JsonPropertyName("resolveRefs")]
		public bool ResolveRefs { get; set; }
		[JsonPropertyName("files")]
		public List<String> Files { get; set; }
	}
}
