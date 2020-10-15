using System.Text.Json.Serialization;
using IncentiveDataLoader.Core;

namespace IncentiveDataLoader.Models
{
	public class IncentiveModel : Record
	{
		[JsonPropertyName("Name")]
		public string Name { get; set; }
		[JsonPropertyName("Apttus_Config2__Sequence__c")]
		public int Sequence { get; set; }
		[JsonPropertyName("Apttus_Config2__EffectiveDate__c")]
		public string StartDate { get; set; }
		[JsonPropertyName("Apttus_Config2__ExpirationDate__c")]
		public string EndDate { get; set; }
		[JsonPropertyName("Apttus_Config2__Status__c")]
		public string Status { get; set; }
		[JsonPropertyName("Apttus_Config2__UseType__c")]
		public string UseType { get; set; }
		[JsonPropertyName(Constants.Namespace + "BenefitLevel__c")]
		public string BenefitLevel { get; set; }
		[JsonPropertyName(Constants.Namespace + "MeasurementLevel__c")]
		public string MeasurementLevel { get; set; }
		[JsonPropertyName(Constants.Namespace + "IncentiveSubtypeId__c")]
		public string IncentiveSubtypeId { get; set; }
		[JsonPropertyName(Constants.Namespace + "AccountId__c")]
		public string AccountId { get; set; }
		[JsonPropertyName(Constants.Namespace + "IncentiveFormulaId__c")]
		public string IncentiveFormulaId { get; set; }
	}
}