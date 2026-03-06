using System;
using System.Collections.Generic;

namespace Valve.Newtonsoft.Json.Utilities
{
	public class AotHelper
	{
		public static void Ensure(Action action)
		{
			if (AotHelper.IsFalse())
			{
				try
				{
					action();
				}
				catch (Exception innerException)
				{
					throw new InvalidOperationException("", innerException);
				}
			}
		}

		public static void EnsureType<T>() where T : new()
		{
			AotHelper.Ensure(delegate
			{
				Activator.CreateInstance<T>();
			});
		}

		public static void EnsureList<T>()
		{
			AotHelper.Ensure(delegate
			{
				List<T> list = new List<T>();
				new HashSet<T>();
				new CollectionWrapper<T>(list);
				new CollectionWrapper<T>(list);
			});
		}

		public static void EnsureDictionary<TKey, TValue>()
		{
			AotHelper.Ensure(delegate
			{
				new Dictionary<TKey, TValue>();
				new DictionaryWrapper<TKey, TValue>(null);
				new DictionaryWrapper<TKey, TValue>(null);
			});
		}

		public static bool IsFalse()
		{
			return AotHelper.s_alwaysFalse;
		}

		private static bool s_alwaysFalse = DateTime.UtcNow.Year < 0;
	}
}
