#region Using Directives

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using IncentiveDataLoader.Core;
using IncentiveDataLoader.Models;
using static System.Console;

#endregion


/*
 TODO::
 0. Update account list file to have 20K accounts to it - @ntayal-conga - DONE
 1. Clean up the Dev Perf 1 env  - @ntayal-conga - DONE
 2. Make use of ParticipantCount settings while adding participants to an incentive
 3. Fix the logic to increment accIndex and prodIndex in while loop on line 61
 4. Have unique list of accounts for each set. The # of accounts we need would be total of participants across all sets for a given run
 5. Merge all appsettings into single file
 6. Have a setting folder path setting at each set level instead of global dataOut
 7. Add logic for participant offset to not repeat the accounts across sets.
*/
namespace IncentiveDataLoader
{
  public class Loader
  {
    private readonly AppSettings _settings;
    string _dataOutPath;
    public Loader(AppSettings settings)
    {
      _settings = settings;
    }

    public void Load()
    {
      var productIds = File.ReadAllLines(Constants.ProductListFileName).ToList();
      var categories = File.ReadAllLines(Constants.CategoryListFileName)
        .Skip(1)
        .Select(CategoryListItem.FromCsv)
        .ToList();

      var accountIds = File.ReadAllLines(Constants.AccountListFileName).ToList();
      var accountOffset = 0;
      foreach (var setting in _settings.LoadSettings)
      {
        var incentiveRecords = new List<IncentiveModel>();
        var ruleSetRecords = new List<PriceRuleSetModel>();
        var ruleSetExtensions = new List<PriceRulesetExtensionModel>();
        var incentiveRulesetMappings = new List<IncentivePriceRulesetMapModel>();
        var priceRules = new List<PriceRuleModel>();
        var priceRuleExtensions = new List<PriceRuleExtensionModel>();
        var priceRuleEntries = new List<PriceRuleEntryModel>();
        var participants = new List<ParticipantModel>();
        var olis = new List<OliModel>();
        _dataOutPath = $"{setting.Setname}";

        var accounts = accountIds
          .Skip(accountOffset)
          .Take(setting.ParticipantCount)
          .ToList();
        accountOffset += setting.ParticipantCount;

        var productsForOli = productIds.Take(setting.ProductCount);
        var productsBelongToCategories = setting
          .CategoryBreakups
          .Select(breakup => categories
            .Where(c => c.Level == breakup.Level)
            .Select(s => s.ProductId)
            .Take(breakup.Count)
            .ToList()
          ).SelectMany(s => s);

        var allOliProducts = productsBelongToCategories.Concat(productsForOli).ToList();
        var accIndex = 0;
        var prodIndex = 0;
        for (var oliIndex = 0; oliIndex < setting.OliCount; oliIndex++)
        {
          var oli = new OliModel
          {
            Attributes = new RecordAttributes
            {
              Type = "Apttus_Config2__OrderLineItem__c",
              ReferenceId = Guid.NewGuid().ToString("N")
            },
            AccountId = $"{accounts[accIndex]}",
            ProductId = $"{allOliProducts[prodIndex]}",
            OrderId = _settings.Configuration.OrderId,
            PricingDate = $"{_settings.Configuration.StartDate.AddDays(20):yyyy-MM-dd}T00:00:00.000Z",
            Quantity = 1
          };
          accIndex++;
          prodIndex++;

          if (accIndex == accounts.Count)
            accIndex = 0;
          if (prodIndex == allOliProducts.Count)
            prodIndex = 0;

          olis.Add(oli);
        }


        for (var index = 1; index <= setting.IncentiveCount; index++)
        {
          Write($"\rIncentive {index}");
          var incentiveModel = new IncentiveModel
          {
            Attributes = new RecordAttributes
            {
              Type = "Apttus_Config2__Incentive__c",
              ReferenceId = Guid.NewGuid().ToString("N")
            },
            Name = $"{setting.Setname}-{index}",
            Sequence = 1,
            StartDate = _settings.Configuration.StartDate.ToString("yyyy-MM-dd"),
            EndDate = _settings.Configuration.EndDate.ToString("yyyy-MM-dd"),
            Status = "New",
            UseType = "Billing",
            BenefitLevel = Constants.IndividualParticipants,
            MeasurementLevel = Constants.IndividualParticipants,
            IncentiveSubtypeId = _settings.Configuration.SubTypeId,
            IncentiveFormulaId = _settings.Configuration.FormulaId,
            AccountId = accountIds[0]
          };

          participants.AddRange(accounts.Select(accountId => new ParticipantModel
          {
            Attributes = new RecordAttributes
            {
              Type = Constants.Namespace + "IncentiveParticipant__c",
              ReferenceId = Guid.NewGuid().ToString("N")
            },
            Account = accountId,
            Incentive = $"@{incentiveModel.Attributes.ReferenceId}",
            StartDate = $"{_settings.Configuration.StartDate:yyyy-MM-dd}T00:00:00.000Z",
            EndDate = $"{_settings.Configuration.EndDate:yyyy-MM-dd}T00:00:00.000Z",
          }));



          for (var productIndex = 1; productIndex <= setting.ProductCount; productIndex++)
          {
            createPriceRuleSet(
              _settings,
              productIndex - 1,
              incentiveModel,
              ruleSetRecords,
              ruleSetExtensions,
              incentiveRulesetMappings,
              priceRules,
              priceRuleExtensions,
              productIds,
              priceRuleEntries,
              Constants.Product);
          }


          foreach (var categoryIds in setting.CategoryBreakups.Select(breakup => categories
            .Where(c => c.Level == breakup.Level)
            .Select(s => s.Id).Take(breakup.Count).ToList()))
          {
            for (var categoryIndex = 1; categoryIndex <= categoryIds.Count; categoryIndex++)
            {
              createPriceRuleSet(
                _settings,
                categoryIndex - 1,
                incentiveModel,
                ruleSetRecords,
                ruleSetExtensions,
                incentiveRulesetMappings,
                priceRules,
                priceRuleExtensions,
                categoryIds,
                priceRuleEntries,
                Constants.Category
                );
            }
          }
          incentiveRecords.Add(incentiveModel);
        }

        try
        {
          Directory.Delete(_dataOutPath, true);
        }
        catch { }
        Directory.CreateDirectory(_dataOutPath);
        WriteLine("");
        WriteLine("Creating Files at: " + _dataOutPath);

        var dataPlanItems = new List<DataPlanModel>();
        var oliDataItems = CreateJsonFiles(olis, "Apttus_Config2__OrderLineItem__c", "oli", 200);
        var incentiveDataItems = CreateJsonFiles(incentiveRecords, "Apttus_Config2__Incentive__c", "incentive", 200);
        var ruleSetDataItems = CreateJsonFiles(ruleSetRecords, "Apttus_Config2__PriceRuleset__c", "rule-set", 200);
        var ruleSetMappingsDataItems = CreateJsonFiles(incentiveRulesetMappings, Constants.Namespace + "IncentivePriceRuleSetMapping__c", "rule-set-incentive-map", 200);
        var ruleSetExtenstionDataItems = CreateJsonFiles(ruleSetExtensions, Constants.Namespace + "PriceRulesetExtension__c", "rule-set-extension", 200);
        var priceRulesDataItems = CreateJsonFiles(priceRules, "Apttus_Config2__PriceRule__c", "rule", 200);
        var priceRuleExtensionDataItems = CreateJsonFiles(priceRuleExtensions, Constants.Namespace + "PriceRuleExtension__c", "rule-extension", 200);
        var priceRuleEntryDataItems = CreateJsonFiles(priceRuleEntries, "Apttus_Config2__PriceRuleEntry__c", "rule-entry", 200);
        var participantsDataItems = CreateJsonFiles(participants, Constants.Namespace + "IncentiveParticipant__c", "participant", 200);

        dataPlanItems.Add(oliDataItems);
        dataPlanItems.Add(incentiveDataItems);
        dataPlanItems.Add(ruleSetDataItems);
        dataPlanItems.Add(ruleSetMappingsDataItems);
        dataPlanItems.Add(ruleSetExtenstionDataItems);
        dataPlanItems.Add(priceRulesDataItems);
        dataPlanItems.Add(priceRuleExtensionDataItems);
        dataPlanItems.Add(priceRuleEntryDataItems);
        dataPlanItems.Add(participantsDataItems);


        string tempPath = Path.Join(_dataOutPath, "plan.json");
        File.WriteAllText(tempPath,
          JsonSerializer.Serialize(dataPlanItems, new JsonSerializerOptions
          {
            WriteIndented = true
          }));
      }

    }

    public DataPlanModel CreateJsonFiles<T>(List<T> records, string objectName, string folderRoot, int size) where T : Record
    {
      var model = new DataPlanModel
      {
        ResolveRefs = true,
        SaveRefs = true,
        SObject = objectName,
        Files = new List<string>()
      };
      foreach (var splitRecords in records.Split(size))
      {
        var filename = $"{Guid.NewGuid():N}.json";
        var filePath = Path.Join(folderRoot, filename.Substring(0, 2), filename.Substring(filename.LastIndexOf('.') - 2, 2), filename);
        Directory.CreateDirectory(Path.GetDirectoryName(Path.Join(this._dataOutPath, filePath)));
        var file = new ImportFile<T> { Records = splitRecords.ToList() };
        File.WriteAllText(Path.Join(this._dataOutPath, filePath),
          JsonSerializer.Serialize(file, new JsonSerializerOptions
          {
            WriteIndented = true
          }));
        model.Files.Add(filePath);
      }

      return model;
    }

    private void createPriceRuleSet(AppSettings settings,
      int productIndex,
      Record incentiveModel,
      ICollection<PriceRuleSetModel> ruleSets,
      ICollection<PriceRulesetExtensionModel> ruleSetExtensions,
      ICollection<IncentivePriceRulesetMapModel> incentiveRulesetMappings,
      ICollection<PriceRuleModel> priceRules,
      ICollection<PriceRuleExtensionModel> priceRuleExtensions,
      IReadOnlyList<string> productIds,
      ICollection<PriceRuleEntryModel> priceRuleEntries,
      string catalogItemType = "Product"
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


      var priceRuleSetMap = new IncentivePriceRulesetMapModel
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
          Type = "Apttus_Config2__PriceRule__c",
          ReferenceId = Guid.NewGuid().ToString("N")
        },
        Name = "B1",
        Active = true,
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
          Type = "Apttus_Config2__PriceRule__c",
          ReferenceId = Guid.NewGuid().ToString("N")
        },
        Name = "Q1",
        Active = true,
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

      var ruleEntry1 = new PriceRuleEntryModel
      {
        Attributes = new RecordAttributes
        {
          Type = $"Apttus_Config2__PriceRuleEntry__c",
          ReferenceId = Guid.NewGuid().ToString("N")
        },
        PriceRuleSetId = $"@{priceRule1.Attributes.ReferenceId}",
        AdjustmentAmount = "10",
        Dimension4Value = null,
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
      var ruleEntry2 = new PriceRuleEntryModel
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
