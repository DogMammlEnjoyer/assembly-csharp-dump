using System;
using System.Diagnostics;
using UnityEngine.Scripting;

namespace Unity.Profiling
{
	[UsedByNativeCode]
	[DebuggerDisplay("Value = {Value}; Count = {Count}")]
	public struct ProfilerRecorderSample
	{
		public long Value
		{
			get
			{
				return this.value;
			}
		}

		public long Count
		{
			get
			{
				return this.count;
			}
		}

		private long value;

		private long count;

		private long refValue;
	}
}
