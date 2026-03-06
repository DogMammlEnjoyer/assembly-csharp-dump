using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.XR.CoreUtils
{
	public static class CollectionExtensions
	{
		public static string Stringify<T>(this ICollection<T> collection)
		{
			CollectionExtensions.k_String.Length = 0;
			int num = collection.Count - 1;
			int num2 = 0;
			foreach (T t in collection)
			{
				CollectionExtensions.k_String.AppendFormat((num2++ == num) ? "{0}" : "{0}, ", t);
			}
			return CollectionExtensions.k_String.ToString();
		}

		private static readonly StringBuilder k_String = new StringBuilder();
	}
}
