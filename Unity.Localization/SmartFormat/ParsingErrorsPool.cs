using System;
using UnityEngine.Localization.SmartFormat.Core.Parsing;
using UnityEngine.Pool;

namespace UnityEngine.Localization.SmartFormat
{
	internal static class ParsingErrorsPool
	{
		public static ParsingErrors Get(Format format)
		{
			ParsingErrors parsingErrors = ParsingErrorsPool.s_Pool.Get();
			parsingErrors.Init(format);
			return parsingErrors;
		}

		public static void Release(ParsingErrors toRelease)
		{
			ParsingErrorsPool.s_Pool.Release(toRelease);
		}

		internal static readonly ObjectPool<ParsingErrors> s_Pool = new ObjectPool<ParsingErrors>(() => new ParsingErrors(), null, delegate(ParsingErrors pe)
		{
			pe.Clear();
		}, null, true, 10, 10000);
	}
}
