using System.Text.Json.Serialization;

namespace IncentiveDataLoader.Models
{
	public class OliModel : Record
	{
		[JsonPropertyName("Apttus_Config2__BillToAccountId__c")]
		public string AccountId { get; set; }
		[JsonPropertyName("Apttus_Config2__ProductId__c")]
		public string ProductId { get; set; }
		[JsonPropertyName("Apttus_Config2__OrderId__c")]
		public string OrderId { get; set; }
		[JsonPropertyName("Apttus_Config2__PricingDate__c")]
		public string PricingDate { get; set; }
		[JsonPropertyName("Apttus_Config2__Quantity__c")]
		public int Quantity { get; set; }
	}
}
