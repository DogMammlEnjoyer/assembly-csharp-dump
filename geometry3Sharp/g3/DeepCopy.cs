using System;
using System.Collections.Generic;
using System.Linq;

namespace g3
{
	public static class DeepCopy
	{
		public static List<T> List<T>(IEnumerable<T> Input) where T : IDuplicatable<T>
		{
			List<T> list = new List<T>();
			foreach (T t in Input)
			{
				list.Add(t.Duplicate());
			}
			return list;
		}

		public static T[] Array<T>(IEnumerable<T> Input) where T : IDuplicatable<T>
		{
			T[] array = new T[Input.Count<T>()];
			int num = 0;
			foreach (T t in Input)
			{
				array[num++] = t.Duplicate();
			}
			return array;
		}
	}
}
