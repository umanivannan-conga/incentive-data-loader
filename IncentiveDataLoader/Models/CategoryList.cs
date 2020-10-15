using System;

namespace IncentiveDataLoader.Models
{
	public class CategoryListItem
	{
		public string Id { get; set; }
		public int Level { get; set; }

		public static CategoryListItem FromCsv(string csvLine)
		{
			string[] values = csvLine.Split(',');

			var item = new CategoryListItem();
			item.Id = values[0];
			item.Level = Convert.ToInt32(values[1]);

			return item;
		}
	}
}
