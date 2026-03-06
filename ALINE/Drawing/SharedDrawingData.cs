using System;
using Unity.Burst;

namespace Drawing
{
	public static class SharedDrawingData
	{
		public static readonly SharedStatic<float> BurstTime = SharedStatic<float>.GetOrCreateUnsafe(4U, 527447541831459905L, -5918529866343830416L);

		private class BurstTimeKey
		{
		}
	}
}
