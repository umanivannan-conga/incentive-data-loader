using System;

namespace IncentiveDataLoader.Models
{
	public class CategoryListItem
	{
		public string Id { get; set; }
		public int Level { get; set; }
		public string ProductId { get; set; }
		public static CategoryListItem FromCsv(string csvLine)
		{
			var values = csvLine.Split(',');

			var item = new CategoryListItem
			{
				Id = values[0],
				ProductId = values[1],
				Level = Convert.ToInt32(values[2])
			};

			return item;
		}
	}
}
