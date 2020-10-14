using System.Text.Json.Serialization;

namespace IncentiveDataLoader.RecordTypes
{
	public class PriceRuleModel : Record
	{
		[JsonPropertyName("Name")]
		public string Name { get; set; }
		public bool Active { get; set; }

	}
}