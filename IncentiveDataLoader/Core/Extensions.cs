using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IncentiveDataLoader.Core
{
	public static class Extensions
	{
		public static IEnumerable<IEnumerable<T>> Split<T>(this List<T> list, int size)
		{
			for (var i = 0; i < (float)list.Count / size; i++)
			{
				yield return list.Skip(i * size).Take(size);
			}
		}

		public static T PickRandom<T>(this IEnumerable<T> source)
		{
			return source.PickRandom(1).Single();
		}

		public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
		{
			return source.Shuffle().Take(count);
		}

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
		{
			return source.OrderBy(x => Guid.NewGuid());
		}
	}
}
