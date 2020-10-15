using System.Text.Json.Serialization;

namespace IncentiveDataLoader.Models
{
	public class PriceRuleModel : Record
	{
		[JsonPropertyName("Name")]
		public string Name { get; set; }
		[JsonPropertyName("Apttus_Config2__Active__c")]
		public bool Active { get; set; }
		[JsonPropertyName("Apttus_Config2__Dimension1Id__c")]
		public string Dimension1Id { get; set; }
		[JsonPropertyName("Apttus_Config2__Dimension2Id__c")]
		public string Dimension2Id { get; set; }
		[JsonPropertyName("Apttus_Config2__RuleType__c")]
		public string RuleType { get; set; }
		[JsonPropertyName("Apttus_Config2__Sequence__c")]
		public int Sequence { get; set; }
		[JsonPropertyName("Apttus_Config2__AdjustmentChargeType__c")]
		public string AdjustmentChargeType { get; set; }
		[JsonPropertyName("Apttus_Config2__AllowRemovalOfAdjustment__c")]
		public bool AllowRemovalOfAdjustment { get; set; }
		[JsonPropertyName("Apttus_Config2__AllowableAction__c")]
		public string AllowableAction { get; set; }
		[JsonPropertyName("Apttus_Config2__BeneficiaryType__c")]
		public string BeneficiaryType { get; set; }
		[JsonPropertyName("Apttus_Config2__BenefitType__c")]
		public string BenefitType { get; set; }
		[JsonPropertyName("Apttus_Config2__CustomPricePointSource__c")]
		public string CustomPricePointSource { get; set; }
		[JsonPropertyName("Apttus_Config2__Dimension1ValueType__c")]
		public string Dimension1ValueType { get; set; }
		[JsonPropertyName("Apttus_Config2__Dimension2ValueType__c")]
		public string Dimension2ValueType { get; set; }
		[JsonPropertyName("Apttus_Config2__StopProcessingMoreRules__c")]
		public bool StopProcessingMoreRules { get; set; }
		[JsonPropertyName("Apttus_Config2__EffectiveDate__c")]
		public string StartDate { get; set; }
		[JsonPropertyName("Apttus_Config2__ExpirationDate__c")]
		public string EndDate { get; set; }
		[JsonPropertyName("Apttus_Config2__RulesetId__c")]
		public string PriceRuleSetId { get; set; }
	}
}