#region Using Directives

using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using IncentiveDataLoader.RecordTypes;
using Microsoft.Extensions.Configuration;
using static System.Console;
#endregion

namespace IncentiveDataLoader
{
	class Program
	{

		static void Main(string[] args)
		{
			IConfiguration config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", true, true)
				.Build();

			var settings = config.GetSection("AppSettings").Get<AppSettings>();

			WriteLine(settings.Configuration.FormulaId);
		}
	}
}
