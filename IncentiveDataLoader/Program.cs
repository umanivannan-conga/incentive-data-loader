#region Using Directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using static System.Console;
#endregion

namespace IncentiveDataLoader
{
	class Program
	{

		static void Main(string[] args)
		{
			WriteLine(DateTime.Now.ToString("G"));
			IConfiguration config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", true, true)
				.Build();

			var settings = config.GetSection("AppSettings").Get<AppSettings>();

			var loader = new Loader();
			loader.Load(settings);
			WriteLine(DateTime.Now.ToString("G"));
		}
	}
}
