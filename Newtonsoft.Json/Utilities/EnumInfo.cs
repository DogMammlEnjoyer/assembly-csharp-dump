using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Utilities
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class EnumInfo
	{
		public EnumInfo(bool isFlags, ulong[] values, string[] names, string[] resolvedNames)
		{
			this.IsFlags = isFlags;
			this.Values = values;
			this.Names = names;
			this.ResolvedNames = resolvedNames;
		}

		public readonly bool IsFlags;

		public readonly ulong[] Values;

		public readonly string[] Names;

		public readonly string[] ResolvedNames;
	}
}
