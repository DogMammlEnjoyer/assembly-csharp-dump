using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Utilities
{
	[NullableContext(2)]
	[Nullable(0)]
	public static class AotHelper
	{
		[NullableContext(1)]
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
				new DefaultContractResolver.EnumerableDictionaryWrapper<TKey, TValue>(null);
			});
		}

		public static bool IsFalse()
		{
			return AotHelper.s_alwaysFalse;
		}

		private static bool s_alwaysFalse = DateTime.UtcNow.Year < 0;
	}
}
