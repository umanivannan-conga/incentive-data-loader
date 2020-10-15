using System;
using System.Text.Json.Serialization;
using IncentiveDataLoader.Core;

namespace IncentiveDataLoader.Models
{
	public class ParticipantModel : Record
	{
		[JsonPropertyName(Constants.Namespace+ "Account__c")]
		public string Account { get; set; }
		[JsonPropertyName(Constants.Namespace + "Incentive__c")]
		public string Incentive { get; set; }
		[JsonPropertyName(Constants.Namespace + "EffectiveDate__c")]
		public string StartDate { get; set; }
		[JsonPropertyName(Constants.Namespace + "ExpirationDate__c")]
		public string EndDate { get; set; }
	}
}
