#region Using Directives
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using IncentiveDataLoader.RecordTypes; 
#endregion

namespace IncentiveDataLoader
{
	public class Loader
	{
		public Loader()
		{
			
		}

		public void Load(AppSettings settings)
		{
			var incentiveRecords = new List<IncentiveModel>();
			var ruleSets = new List<PriceRuleSetModel>();

			foreach (var setting in settings.LoadSettings)
			{
				for (var index = 0; index < setting.IncentiveCount; index++)
				{
					incentiveRecords.Add(new IncentiveModel
					{
						Attributes = new RecordAttributes
						{
							Type = "Apttus_Config2__Incentive__c",
							ReferenceId = $"IncentiveRef{index}",
						},
						Name = $"Incentive {index}",
						Sequence = 1,
						StartDate = "2020-10-01",
						EndDate = "2020-10-30",
						Status = "New",
						UseType = "Billing",
						BenefitLevel = "Individual Participants",
						MeasurementLevel = "Individual Participants",
						IncentiveSubtypeId = "a4C5B0000007iJhUAI",
						IncentiveFormulaId = "a485B0000001YXZQA2"

					});
				}	
			}

			for (var incNumber = 1; incNumber <= settings.; incNumber++)
			{
				incentiveRecords.Add(new IncentiveModel
				{
					Attributes = new RecordAttributes
					{
						Type = $"Apttus_Config2__Incentive__c",
						ReferenceId = $"IncentiveRef{incNumber}",
					},
					Name = $"Incentive {incNumber}",
					Sequence = 1,
					StartDate = "2020-10-01",
					EndDate = "2020-10-30",
					Status = "New",
					UseType = "Billing",
					BenefitLevel = "Individual Participants",
					MeasurementLevel = "Individual Participants",
					IncentiveSubtypeId = "a4C5B0000007iJhUAI",
					IncentiveFormulaId = "a485B0000001YXZQA2"

				});


				for (var ruleSetNumber = 1; ruleSetNumber <= ruleSetCount; ruleSetNumber++)
				{
					ruleSets.Add(new PriceRuleSetModel
					{
						Attributes = new RecordAttributes
						{
							Type = $"Apttus_Config2__PriceRuleset__c",
							ReferenceId = $"PriceRulesetRef{incNumber.ToString()?.PadLeft(10, '0')}{ruleSetNumber}"
						},
						Name = $"PRS {ruleSetNumber.ToString()?.PadLeft(10, '0')}",
						Sequence = ruleSetNumber,
						StopProcessingMoreRule = false,
						StartDate = "2020-10-01T00:00:00.000Z",
						EndDate = "2020-10-30T00:00:00.000Z",
						IncentiveId = $"@IncentiveRef{incNumber}"
					});

				}

			}

			var file = new ImportFile<IncentiveModel> { Records = incentiveRecords };

			var ruleSetFile = new ImportFile<PriceRuleSetModel> { Records = ruleSets };

			File.WriteAllText("incentive.json", JsonSerializer.Serialize(file, new JsonSerializerOptions { WriteIndented = true }));
			File.WriteAllText("priceRuleSet.json", JsonSerializer.Serialize(ruleSetFile, new JsonSerializerOptions { WriteIndented = true }));
		}
	}
}
