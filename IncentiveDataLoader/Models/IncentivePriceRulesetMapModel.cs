using System.Text.Json.Serialization;
using IncentiveDataLoader.Core;

namespace IncentiveDataLoader.Models
{
	public class IncentivePriceRulesetMapModel : Record
	{
		[JsonPropertyName(Constants.Namespace + "Incentive__c")]
		public string IncentiveId { get; set; }
		[JsonPropertyName(Constants.Namespace + "PriceRuleSet__c")]
		public string PriceRuleSetId { get; set; }
	}
}
