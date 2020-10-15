using System.Text.Json.Serialization;

namespace IncentiveDataLoader.Models
{
	public class PriceRuleSetModel : Record
	{
		[JsonPropertyName("Name")]
		public string Name { get; set; }
		[JsonPropertyName("Apttus_Config2__EffectiveDate__c")]
		public string StartDate { get; set; }
		[JsonPropertyName("Apttus_Config2__ExpirationDate__c")]
		public string EndDate { get; set; }
		[JsonPropertyName("Apttus_Config2__StopProcessingMoreRules__c")]
		public bool StopProcessingMoreRule { get; set; }
		[JsonPropertyName("Apttus_Config2__Type__c")]
		public string Type { get; set; }

		[JsonPropertyName("Apttus_Config2__Sequence__c")]
		public int Sequence { get; set; }
		[JsonPropertyName("Apttus_Config2__IncentiveId__c")]
		public string IncentiveId { get; set; }
	}
}