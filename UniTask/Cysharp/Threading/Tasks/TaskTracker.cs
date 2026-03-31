using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Cysharp.Threading.Tasks.Internal;

namespace Cysharp.Threading.Tasks
{
	public static class TaskTracker
	{
		[Conditional("UNITY_EDITOR")]
		public static void TrackActiveTask(IUniTaskSource task, int skipFrame)
		{
		}

		[Conditional("UNITY_EDITOR")]
		public static void RemoveTracking(IUniTaskSource task)
		{
		}

		public static bool CheckAndResetDirty()
		{
			bool result = TaskTracker.dirty;
			TaskTracker.dirty = false;
			return result;
		}

		public static void ForEachActiveTask(Action<int, string, UniTaskStatus, DateTime, string> action)
		{
			List<KeyValuePair<IUniTaskSource, ValueTuple<string, int, DateTime, string>>> obj = TaskTracker.listPool;
			lock (obj)
			{
				int num = TaskTracker.tracking.ToList(ref TaskTracker.listPool, false);
				try
				{
					for (int i = 0; i < num; i++)
					{
						action(TaskTracker.listPool[i].Value.Item2, TaskTracker.listPool[i].Value.Item1, TaskTracker.listPool[i].Key.UnsafeGetStatus(), TaskTracker.listPool[i].Value.Item3, TaskTracker.listPool[i].Value.Item4);
						TaskTracker.listPool[i] = default(KeyValuePair<IUniTaskSource, ValueTuple<string, int, DateTime, string>>);
					}
				}
				catch
				{
					TaskTracker.listPool.Clear();
					throw;
				}
			}
		}

		private static void TypeBeautify(Type type, StringBuilder sb)
		{
			if (type.IsNested)
			{
				sb.Append(type.DeclaringType.Name.ToString());
				sb.Append(".");
			}
			if (type.IsGenericType)
			{
				int num = type.Name.IndexOf("`");
				if (num != -1)
				{
					sb.Append(type.Name.Substring(0, num));
				}
				else
				{
					sb.Append(type.Name);
				}
				sb.Append("<");
				bool flag = true;
				foreach (Type type2 in type.GetGenericArguments())
				{
					if (!flag)
					{
						sb.Append(", ");
					}
					flag = false;
					TaskTracker.TypeBeautify(type2, sb);
				}
				sb.Append(">");
				return;
			}
			sb.Append(type.Name);
		}

		[TupleElementNames(new string[]
		{
			"formattedType",
			"trackingId",
			"addTime",
			"stackTrace"
		})]
		private static List<KeyValuePair<IUniTaskSource, ValueTuple<string, int, DateTime, string>>> listPool = new List<KeyValuePair<IUniTaskSource, ValueTuple<string, int, DateTime, string>>>();

		[TupleElementNames(new string[]
		{
			"formattedType",
			"trackingId",
			"addTime",
			"stackTrace"
		})]
		private static readonly WeakDictionary<IUniTaskSource, ValueTuple<string, int, DateTime, string>> tracking = new WeakDictionary<IUniTaskSource, ValueTuple<string, int, DateTime, string>>(4, 0.75f, null);

		private static bool dirty;
	}
}
