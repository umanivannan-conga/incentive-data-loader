using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IncentiveDataLoader.Core
{
public static	class Extensions
	{
		public static IEnumerable<IEnumerable<T>> Split<T>(this List<T> list, int size)
		{
			for (var i = 0; i < (float)list.Count / size; i++)
			{
				yield return list.Skip(i * size).Take(size);
			}
		}
  }
}
