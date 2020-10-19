#region Using Directives

using System.Text.Json.Serialization;

#endregion

namespace IncentiveDataLoader.Models
{
	public class PriceRuleEntryModel : Record
	{
		[JsonPropertyName("Apttus_Config2__PriceRuleId__c")]
		public string PriceRuleSetId { get; set; }
		[JsonPropertyName("Apttus_Config2__AdjustmentAmount__c")]
		public string AdjustmentAmount { get; set; }
		[JsonPropertyName("Apttus_Config2__Dimension4Value__c")]
		public string Dimension4Value { get; set; }
		[JsonPropertyName("Apttus_Config2__Dimension2ListValue__c")]
		public string Dimension2ListValue { get; set; }
		[JsonPropertyName("Apttus_Config2__Dimension1ListValue__c")]
		public string Dimension1ListValue { get; set; }
		[JsonPropertyName("Apttus_Config2__AdjustmentValueType__c")]
		public string AdjustmentValueType { get; set; }
		[JsonPropertyName("Apttus_Config2__AdjustmentType__c")]
		public string AdjustmentType { get; set; }
		[JsonPropertyName("Apttus_Config2__Sequence__c")]
		public string Sequence { get; set; }
	}
}