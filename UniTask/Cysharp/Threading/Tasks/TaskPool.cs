using System;
using System.Collections.Generic;

namespace Cysharp.Threading.Tasks
{
	public static class TaskPool
	{
		static TaskPool()
		{
			try
			{
				string environmentVariable = Environment.GetEnvironmentVariable("UNITASK_MAX_POOLSIZE");
				int maxPoolSize;
				if (environmentVariable != null && int.TryParse(environmentVariable, out maxPoolSize))
				{
					TaskPool.MaxPoolSize = maxPoolSize;
					return;
				}
			}
			catch
			{
			}
			TaskPool.MaxPoolSize = int.MaxValue;
		}

		public static void SetMaxPoolSize(int maxPoolSize)
		{
			TaskPool.MaxPoolSize = maxPoolSize;
		}

		public static IEnumerable<ValueTuple<Type, int>> GetCacheSizeInfo()
		{
			Dictionary<Type, Func<int>> obj = TaskPool.sizes;
			lock (obj)
			{
				foreach (KeyValuePair<Type, Func<int>> keyValuePair in TaskPool.sizes)
				{
					yield return new ValueTuple<Type, int>(keyValuePair.Key, keyValuePair.Value());
				}
				Dictionary<Type, Func<int>>.Enumerator enumerator = default(Dictionary<Type, Func<int>>.Enumerator);
			}
			obj = null;
			yield break;
			yield break;
		}

		public static void RegisterSizeGetter(Type type, Func<int> getSize)
		{
			Dictionary<Type, Func<int>> obj = TaskPool.sizes;
			lock (obj)
			{
				TaskPool.sizes[type] = getSize;
			}
		}

		internal static int MaxPoolSize;

		private static Dictionary<Type, Func<int>> sizes = new Dictionary<Type, Func<int>>();
	}
}
