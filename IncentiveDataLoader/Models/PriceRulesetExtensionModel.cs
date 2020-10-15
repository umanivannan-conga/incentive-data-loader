#region Using Directives

using System.Text.Json.Serialization;
using IncentiveDataLoader.Core;

#endregion

namespace IncentiveDataLoader.Models
{
	public class PriceRulesetExtensionModel : Record
	{
		[JsonPropertyName("Name")]
		public string Name { get; set; }
		[JsonPropertyName(Constants.Namespace + "TierCount__c")]
		public int TierCount { get; set; }
		[JsonPropertyName(Constants.Namespace + "PriceRulesetId__c")]
		public string PriceRulesetId { get; set; }
		[JsonPropertyName(Constants.Namespace + "Criteria__c")]
		public string Criteria { get; set; }

	}
}
