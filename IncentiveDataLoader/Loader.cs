#region Using Directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using IncentiveDataLoader.Core;
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
			


			List<String> productIds = File.ReadAllLines(Constants.ProductListFileName).ToList();

			foreach (var setting in settings.LoadSettings)
			{
				var priceRulesetCount = setting.ProductCount + setting.CategoryBreakups.Sum(item => item.Count);

				for (var index = 1; index <= setting.IncentiveCount; index++)
				{
					var incentiveModel= new IncentiveModel
					{
						Attributes = new RecordAttributes
						{
							Type = "Apttus_Config2__Incentive__c",
							ReferenceId = Guid.NewGuid().ToString(),
						},
						Name = $"{settings.Configuration.IncentiveNamePrefix}-{index}",
						Sequence = 1,
						StartDate = "2020-10-01",
						EndDate = "2020-10-30",
						Status = "New",
						UseType = "Billing",
						BenefitLevel = "Individual Participants",
						MeasurementLevel = "Individual Participants",
						IncentiveSubtypeId = "a4C5B0000007iJhUAI",
						IncentiveFormulaId = "a485B0000001YXZQA2"

					};

					for (int productIndex = 1; productIndex <= setting.ProductCount; productIndex++)
					{
						var priceRuleSet = new PriceRuleSetModel
						{
							Attributes = new RecordAttributes
							{
								Type = $"Apttus_Config2__PriceRuleset__c",
								ReferenceId = Guid.NewGuid().ToString()
							},
							Name = $"PRS-{productIndex.ToString()?.PadLeft(10, '0')}",
							Sequence = productIndex,
							StopProcessingMoreRule = false,
							StartDate = "2020-10-01T00:00:00.000Z",
							EndDate = "2020-10-30T00:00:00.000Z",
							IncentiveId = $"@{incentiveModel.Attributes.ReferenceId}"
						};
					}


					incentiveRecords.Add(incentiveModel);
					incentiveRecords.Add(new IncentiveModel());

				}

				

			}

			

			var file = new ImportFile<IncentiveModel> { Records = incentiveRecords };

			var ruleSetFile = new ImportFile<PriceRuleSetModel> { Records = ruleSets };

			File.WriteAllText("incentive.json", JsonSerializer.Serialize(file, new JsonSerializerOptions { WriteIndented = true }));
			File.WriteAllText("priceRuleSet.json", JsonSerializer.Serialize(ruleSetFile, new JsonSerializerOptions { WriteIndented = true }));
		}
	}
}
