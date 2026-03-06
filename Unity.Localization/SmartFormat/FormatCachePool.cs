using System;
using UnityEngine.Localization.SmartFormat.Core.Formatting;
using UnityEngine.Localization.SmartFormat.Core.Parsing;
using UnityEngine.Pool;

namespace UnityEngine.Localization.SmartFormat
{
	internal static class FormatCachePool
	{
		public static FormatCache Get(Format format)
		{
			FormatCache formatCache = FormatCachePool.s_Pool.Get();
			formatCache.Format = format;
			return formatCache;
		}

		public static PooledObject<FormatCache> Get(Format format, out FormatCache value)
		{
			PooledObject<FormatCache> result = FormatCachePool.s_Pool.Get(out value);
			value.Format = format;
			return result;
		}

		public static void Release(FormatCache toRelease)
		{
			FormatCachePool.s_Pool.Release(toRelease);
		}

		internal static readonly ObjectPool<FormatCache> s_Pool = new ObjectPool<FormatCache>(() => new FormatCache(), null, delegate(FormatCache fc)
		{
			FormatItemPool.ReleaseFormat(fc.Format);
			fc.Format = null;
			fc.Table = null;
			fc.CachedObjects.Clear();
			fc.VariableTriggers.Clear();
			fc.LocalVariables = null;
		}, null, true, 10, 10000);
	}
}
