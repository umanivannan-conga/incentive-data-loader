using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IncentiveDataLoader
{
	public class AppSettings
	{
		[JsonPropertyName("Configuration")]
		public Configuration Configuration { get; set; }
		[JsonPropertyName("LoadSettings")]
		public List<LoadSettings> LoadSettings { get; set; }
	}

	public class Configuration
	{
		[JsonPropertyName("SubTypeId")]
		public string SubTypeId { get; set; }
		[JsonPropertyName("FormulaId")]
		public string FormulaId { get; set; }
		[JsonPropertyName("IncentiveNamePrefix")]
		public string IncentiveNamePrefix { get; set; }
		[JsonPropertyName("StartDate")]
		public DateTime StartDate { get; set; }
		[JsonPropertyName("EndDate")]
		public DateTime EndDate { get; set; }
		[JsonPropertyName("OrderId")]
		public string OrderId { get; set; }
	}

	public class LoadSettings
	{
		[JsonPropertyName("IncentiveCount")]
		public int IncentiveCount { get; set; }
		[JsonPropertyName("ProductCount")]
		public int ProductCount { get; set; }
		[JsonPropertyName("ParticipantCount")]
		public int ParticipantCount { get; set; }
		[JsonPropertyName("OliCount")]
		public int OliCount { get; set; }
		[JsonPropertyName("SetName")] 
		public string Setname { get; set; }
		[JsonPropertyName("CategoryBreakups")]
		public List<CategoryBreakup> CategoryBreakups { get; set; }
	}

	public class CategoryBreakup
	{
		[JsonPropertyName("Level")]
		public int Level { get; set; }
		[JsonPropertyName("Count")]
		public int Count { get; set; }
	}
}
