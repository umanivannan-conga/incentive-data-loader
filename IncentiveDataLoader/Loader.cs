#region Using Directives

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using IncentiveDataLoader.Core;
using IncentiveDataLoader.Models;
using Microsoft.Extensions.FileSystemGlobbing;

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
			var ruleSetExtensions = new List<PriceRulesetExtensionModel>();
			var incentiveRulesetMappings = new List<IncentivePriceRulesetMap>();
			var priceRules = new List<PriceRuleModel>();
			var priceRuleExtensions = new List<PriceRuleExtensionModel>();
			var priceRuleEntries = new List<PriceRuleEntry>();
			var participants = new List<ParticipantModel>();

			List<String> productIds = File.ReadAllLines(Constants.ProductListFileName).ToList();
			List<CategoryListItem> categories = File.ReadAllLines(Constants.CategoryListFileName)
				.Skip(1)
				.Select(CategoryListItem.FromCsv)
				.ToList();

			List<String> accountIds = File.ReadAllLines(Constants.AccountListFileName).ToList();

			foreach (var setting in settings.LoadSettings)
			{
				var priceRulesetCount = setting.ProductCount + setting.CategoryBreakups.Sum(item => item.Count);


				for (var index = 1; index <= setting.IncentiveCount; index++)
				{
					var incentiveModel = new IncentiveModel
					{
						Attributes = new RecordAttributes
						{
							Type = "Apttus_Config2__Incentive__c",
							ReferenceId = Guid.NewGuid().ToString("N")
						},
						Name = $"{settings.Configuration.IncentiveNamePrefix}-{index}",
						Sequence = 1,
						StartDate = settings.Configuration.StartDate.ToString("yyyy-MM-dd"),
						EndDate = settings.Configuration.EndDate.ToString("yyyy-MM-dd"),
						Status = "New",
						UseType = "Billing",
						BenefitLevel = Constants.IndividualParticipants,
						MeasurementLevel = Constants.IndividualParticipants,
						IncentiveSubtypeId = settings.Configuration.SubTypeId,
						IncentiveFormulaId = settings.Configuration.FormulaId,
						AccountId = accountIds[0]
					};

					foreach (var accountId in accountIds)
					{

						participants.Add(new ParticipantModel()
						{
							Attributes = new RecordAttributes
							{
								Type = Constants.Namespace + "IncentiveParticipant__c",
								ReferenceId = Guid.NewGuid().ToString("N")
							},
							Account = accountId,
							Incentive = incentiveModel.Attributes.ReferenceId
						});
					}

					for (var productIndex = 1; productIndex <= setting.ProductCount; productIndex++)
					{
						createPriceRuleSet(settings, productIndex - 1, incentiveModel, ruleSets, ruleSetExtensions, incentiveRulesetMappings, priceRules, priceRuleExtensions, productIds, priceRuleEntries);
					}


					foreach (var breakup in setting.CategoryBreakups)
					{
						var categoryIds = categories.Where(c => c.Level == breakup.Level)
							.Select(s => s.Id)
							.Take(breakup.Count)
							.ToList();

						for (int categoryIndex = 1; categoryIndex <= categoryIds.Count; categoryIndex++)
						{
							createPriceRuleSet(settings, categoryIndex - 1, incentiveModel, ruleSets, ruleSetExtensions, incentiveRulesetMappings, priceRules, priceRuleExtensions, categoryIds, priceRuleEntries, "Category");
						}


					}


					incentiveRecords.Add(incentiveModel);
				}
			}
			Directory.CreateDirectory("data");
			List<DataPlanModel> dataPlanItems = new List<DataPlanModel>();


			var fileIndex = 1;
			var filesList = new List<String>();
			foreach (var records in incentiveRecords.Split(200))
			{
				var filename = $"incentive{fileIndex++}.json";
				var file = new ImportFile<IncentiveModel> { Records = records.ToList() };
				File.WriteAllText($@"data\{filename}",
					JsonSerializer.Serialize(file, new JsonSerializerOptions
					{
						WriteIndented = true
					}));
				filesList.Add(filename);
			}
			dataPlanItems.Add(new DataPlanModel()
			{
				Files = filesList,
				ResolveRefs = true,
				SaveRefs = true,
				SObject = "Apttus_Config2__Incentive__c"
			});

			fileIndex = 1;
			filesList = new List<String>();
			foreach (var records in ruleSets.Split(25))
			{
				var filename = $"priceRuleSet{fileIndex++}.json";
				var file = new ImportFile<PriceRuleSetModel> { Records = records.ToList() };
				File.WriteAllText($@"data\{filename}",
					JsonSerializer.Serialize(file, new JsonSerializerOptions
					{
						WriteIndented = true
					}));
				filesList.Add(filename);
			}
			dataPlanItems.Add(new DataPlanModel()
			{
				Files = filesList,
				ResolveRefs = true,
				SaveRefs = true,
				SObject = "Apttus_Config2__PriceRuleset__c"
			});



			fileIndex = 1;
			filesList = new List<String>();
			foreach (var records in incentiveRulesetMappings.Split(200))
			{
				var filename = $"priceRuleSetMapping{fileIndex++}.json";
				var file = new ImportFile<IncentivePriceRulesetMap> { Records = records.ToList() };
				File.WriteAllText($@"data\{filename}",
					JsonSerializer.Serialize(file, new JsonSerializerOptions
					{
						WriteIndented = true
					}));
				filesList.Add(filename);
			}
			dataPlanItems.Add(new DataPlanModel()
			{
				Files = filesList,
				ResolveRefs = true,
				SaveRefs = true,
				SObject = "Apttus_CIM__IncentivePriceRuleSetMapping__c"
			});


			fileIndex = 1;
			filesList = new List<String>();
			foreach (var records in ruleSetExtensions.Split(200))
			{
				var filename = $"ruleSetExtensions{fileIndex++}.json";
				var file = new ImportFile<PriceRulesetExtensionModel> { Records = records.ToList() };
				File.WriteAllText($@"data\{filename}",
					JsonSerializer.Serialize(file, new JsonSerializerOptions
					{
						WriteIndented = true
					}));
				filesList.Add(filename);
			}
			dataPlanItems.Add(new DataPlanModel()
			{
				Files = filesList,
				ResolveRefs = true,
				SaveRefs = true,
				SObject = Constants.Namespace + "PriceRulesetExtension__c"
			});

			fileIndex = 1;
			filesList = new List<String>();
			foreach (var records in priceRules.Split(25))
			{
				var filename = $"priceRules{fileIndex++}.json";
				var file = new ImportFile<PriceRuleModel> { Records = records.ToList() };
				File.WriteAllText($@"data\{filename}",
					JsonSerializer.Serialize(file, new JsonSerializerOptions
					{
						WriteIndented = true
					}));
				filesList.Add(filename);
			}
			dataPlanItems.Add(new DataPlanModel()
			{
				Files = filesList,
				ResolveRefs = true,
				SaveRefs = true,
				SObject = "Apttus_Config2__PriceRule__c"
			});



			fileIndex = 1;
			filesList = new List<String>();
			foreach (var records in priceRuleExtensions.Split(200))
			{
				var filename = $"priceRuleExtensions{fileIndex++}.json";
				var file = new ImportFile<PriceRuleExtensionModel> { Records = records.ToList() };
				File.WriteAllText($@"data\{filename}",
					JsonSerializer.Serialize(file, new JsonSerializerOptions
					{
						WriteIndented = true
					}));
				filesList.Add(filename);
			}
			dataPlanItems.Add(new DataPlanModel()
			{
				Files = filesList,
				ResolveRefs = true,
				SaveRefs = true,
				SObject = Constants.Namespace + "PriceRuleExtension__c"
			});



			fileIndex = 1;
			filesList = new List<String>();
			foreach (var records in priceRuleEntries.Split(25))
			{
				var filename = $"priceRuleEntries{fileIndex++}.json";
				var file = new ImportFile<PriceRuleEntry> { Records = records.ToList() };
				File.WriteAllText($@"data\{filename}",
					JsonSerializer.Serialize(file, new JsonSerializerOptions
					{
						WriteIndented = true
					}));
				filesList.Add(filename);
			}
			dataPlanItems.Add(new DataPlanModel()
			{
				Files = filesList,
				ResolveRefs = true,
				SaveRefs = true,
				SObject = "Apttus_Config2__PriceRuleEntry__c"
			});

			fileIndex = 1;
			filesList = new List<String>();
			foreach (var records in participants.Split(200))
			{
				var filename = $"participants{fileIndex++}.json";
				var file = new ImportFile<ParticipantModel> { Records = records.ToList() };
				File.WriteAllText($@"data\{filename}",
					JsonSerializer.Serialize(file, new JsonSerializerOptions
					{
						WriteIndented = true
					}));
				filesList.Add(filename);
			}
			dataPlanItems.Add(new DataPlanModel()
			{
				Files = filesList,
				ResolveRefs = true,
				SaveRefs = true,
				SObject = Constants.Namespace + "IncentiveParticipant__c"
			});

			File.WriteAllText($@"data\plan.json",
				JsonSerializer.Serialize(dataPlanItems, new JsonSerializerOptions
				{
					WriteIndented = true
				}));
		}

		private void createPriceRuleSet(AppSettings settings,
			int productIndex,
			IncentiveModel incentiveModel,
			List<PriceRuleSetModel> ruleSets,
			List<PriceRulesetExtensionModel> ruleSetExtensions,
			List<IncentivePriceRulesetMap> incentiveRulesetMappings,
			List<PriceRuleModel> priceRules,
			List<PriceRuleExtensionModel> priceRuleExtensions,
			List<string> productIds,
			List<PriceRuleEntry> priceRuleEntries,
			String catalogItemType = "Product"
			)
		{
			var priceRuleSet = new PriceRuleSetModel
			{
				Attributes = new RecordAttributes
				{
					Type = $"Apttus_Config2__PriceRuleset__c",
					ReferenceId = Guid.NewGuid().ToString("N")
				},
				Name = $"PRS-{productIndex.ToString()?.PadLeft(10, '0')}",
				Sequence = productIndex,
				StopProcessingMoreRule = false,
				StartDate = $"{settings.Configuration.StartDate:yyyy-MM-dd}T00:00:00.000Z",
				EndDate = $"{settings.Configuration.EndDate:yyyy-MM-dd}T00:00:00.000Z",
				Type = "Incentive"
			};
			ruleSets.Add(priceRuleSet);

			var priceRuleSetExtension = new PriceRulesetExtensionModel
			{
				Attributes = new RecordAttributes()
				{
					Type = $"{Constants.Namespace}PriceRulesetExtension__c",
					ReferenceId = Guid.NewGuid().ToString("N")
				},
				Name = priceRuleSet.Name,
				Criteria = "",
				PriceRulesetId = $"@{priceRuleSet.Attributes.ReferenceId}",
				TierCount = 1, //TODO: hard coded for now
			};
			ruleSetExtensions.Add(priceRuleSetExtension);


			var priceRuleSetMap = new IncentivePriceRulesetMap
			{
				Attributes = new RecordAttributes
				{
					Type = $"{Constants.Namespace}IncentivePriceRuleSetMapping__c",
					ReferenceId = Guid.NewGuid().ToString("N")
				},

				IncentiveId = $"@{incentiveModel.Attributes.ReferenceId}",
				PriceRuleSetId = $"@{priceRuleSet.Attributes.ReferenceId}"
			};
			incentiveRulesetMappings.Add(priceRuleSetMap);

			var priceRule1 = new PriceRuleModel
			{
				Attributes = new RecordAttributes
				{
					Type = $"Apttus_Config2__PriceRule__c",
					ReferenceId = Guid.NewGuid().ToString("N")
				},
				Name = "B1",
				Active = true,
				Dimension1Id = settings.Configuration.CategoryDimensionId,
				Dimension2Id = settings.Configuration.ProductDimensionId,
				RuleType = Constants.Benefit,
				Sequence = 1,
				AdjustmentChargeType = "Adjustment",
				AllowRemovalOfAdjustment = false,
				AllowableAction = "Unrestricted",
				BeneficiaryType = "Account",
				BenefitType = "Price Only",
				CustomPricePointSource = "AdjustedPrice",
				Dimension1ValueType = "Discrete",
				Dimension2ValueType = "Discrete",
				StopProcessingMoreRules = false,
				StartDate = settings.Configuration.StartDate.ToString("yyyy-MM-dd"),
				EndDate = settings.Configuration.EndDate.ToString("yyyy-MM-dd"),
				PriceRuleSetId = $"@{priceRuleSet.Attributes.ReferenceId}"
			};
			var priceRule2 = new PriceRuleModel
			{
				Attributes = new RecordAttributes
				{
					Type = $"Apttus_Config2__PriceRule__c",
					ReferenceId = Guid.NewGuid().ToString("N")
				},
				Name = "Q1",
				Active = true,
				Dimension1Id = settings.Configuration.CategoryDimensionId,
				Dimension2Id = settings.Configuration.ProductDimensionId,
				RuleType = Constants.Qualification,
				Sequence = 1,
				AdjustmentChargeType = "Adjustment",
				AllowRemovalOfAdjustment = false,
				AllowableAction = "Unrestricted",
				BeneficiaryType = "Account",
				BenefitType = "Price Only",
				CustomPricePointSource = "AdjustedPrice",
				Dimension1ValueType = "Discrete",
				Dimension2ValueType = "Discrete",
				StopProcessingMoreRules = false,
				StartDate = settings.Configuration.StartDate.ToString("yyyy-MM-dd"),
				EndDate = settings.Configuration.EndDate.ToString("yyyy-MM-dd"),
				PriceRuleSetId = $"@{priceRuleSet.Attributes.ReferenceId}"
			};

			priceRules.Add(priceRule1);
			priceRules.Add(priceRule2);

			var priceRuleExtension1 = new PriceRuleExtensionModel
			{
				Attributes = new RecordAttributes
				{
					Type = $"{Constants.Namespace}PriceRuleExtension__c",
					ReferenceId = Guid.NewGuid().ToString("N")
				},
				AggregateType = Constants.Billing,
				AlternateAmount = "",
				PriceRuleId = $"@{priceRule1.Attributes.ReferenceId}"
			};
			var priceRuleExtension2 = new PriceRuleExtensionModel
			{
				Attributes = new RecordAttributes
				{
					Type = $"{Constants.Namespace}PriceRuleExtension__c",
					ReferenceId = Guid.NewGuid().ToString("N")
				},
				AggregateType = Constants.Reporting,
				AlternateAmount = "",
				PriceRuleId = $"@{priceRule2.Attributes.ReferenceId}"
			};

			priceRuleExtensions.Add(priceRuleExtension1);
			priceRuleExtensions.Add(priceRuleExtension2);

			var ruleEntry1 = new PriceRuleEntry
			{
				Attributes = new RecordAttributes
				{
					Type = $"Apttus_Config2__PriceRuleEntry__c",
					ReferenceId = Guid.NewGuid().ToString("N")
				},
				PriceRuleSetId = $"@{priceRule1.Attributes.ReferenceId}",
				AdjustmentAmount = "10",
				Dimension4Value = null,
				Dimension2ListValue = productIds[productIndex],
				Dimension1ListValue = null,
				AdjustmentValueType = "Constant",
				AdjustmentType = null,
				Sequence = "1"
			};
			var ruleEntry2 = new PriceRuleEntry
			{
				Attributes = new RecordAttributes
				{
					Type = $"Apttus_Config2__PriceRuleEntry__c",
					ReferenceId = Guid.NewGuid().ToString("N")
				},
				PriceRuleSetId = $"@{priceRule2.Attributes.ReferenceId}",
				AdjustmentAmount = null,
				Dimension4Value = "10",
				Dimension2ListValue = catalogItemType.ToLower(CultureInfo.InvariantCulture) == "product"
					? productIds[productIndex]
				: null,
				Dimension1ListValue = catalogItemType.ToLower(CultureInfo.InvariantCulture) == "category"
					? productIds[productIndex]
				: null,
				AdjustmentValueType = "Constant",
				AdjustmentType = null,
				Sequence = "1"
			};

			priceRuleEntries.Add(ruleEntry1);
			priceRuleEntries.Add(ruleEntry2);
		}
	}
}
