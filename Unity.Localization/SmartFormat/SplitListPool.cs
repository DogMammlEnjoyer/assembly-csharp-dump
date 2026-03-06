using System;
using System.Collections.Generic;
using UnityEngine.Localization.SmartFormat.Core.Parsing;
using UnityEngine.Pool;

namespace UnityEngine.Localization.SmartFormat
{
	internal static class SplitListPool
	{
		public static Format.SplitList Get(Format format, List<int> splits)
		{
			Format.SplitList splitList = SplitListPool.s_Pool.Get();
			splitList.Init(format, splits);
			return splitList;
		}

		public static void Release(Format.SplitList toRelease)
		{
			SplitListPool.s_Pool.Release(toRelease);
		}

		internal static readonly ObjectPool<Format.SplitList> s_Pool = new ObjectPool<Format.SplitList>(() => new Format.SplitList(), null, delegate(Format.SplitList sl)
		{
			sl.Clear();
		}, null, true, 10, 10000);
	}
}
