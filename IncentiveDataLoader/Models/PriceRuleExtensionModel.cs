using System.Text.Json.Serialization;
using IncentiveDataLoader.Core;

namespace IncentiveDataLoader.Models
{
	public class PriceRuleExtensionModel:Record
	{
		[JsonPropertyName(Constants.Namespace + "AggregateType__c")]
		public string AggregateType { get; set; }
		[JsonPropertyName(Constants.Namespace + "AlternateAmount__c")]
		public string AlternateAmount { get; set; }
		[JsonPropertyName(Constants.Namespace + "PriceRuleId__c")]
		public string PriceRuleId { get; set; }
	}
}
