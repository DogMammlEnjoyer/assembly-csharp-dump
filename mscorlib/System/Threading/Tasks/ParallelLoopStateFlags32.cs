using System;

namespace System.Threading.Tasks
{
	internal class ParallelLoopStateFlags32 : ParallelLoopStateFlags
	{
		internal int LowestBreakIteration
		{
			get
			{
				return this._lowestBreakIteration;
			}
		}

		internal long? NullableLowestBreakIteration
		{
			get
			{
				if (this._lowestBreakIteration == 2147483647)
				{
					return null;
				}
				long value = (long)this._lowestBreakIteration;
				if (IntPtr.Size >= 8)
				{
					return new long?(value);
				}
				return new long?(Interlocked.Read(ref value));
			}
		}

		internal bool ShouldExitLoop(int CallerIteration)
		{
			int loopStateFlags = base.LoopStateFlags;
			return loopStateFlags != 0 && ((loopStateFlags & 13) != 0 || ((loopStateFlags & 2) != 0 && CallerIteration > this.LowestBreakIteration));
		}

		internal bool ShouldExitLoop()
		{
			int loopStateFlags = base.LoopStateFlags;
			return loopStateFlags != 0 && (loopStateFlags & 9) != 0;
		}

		internal volatile int _lowestBreakIteration = int.MaxValue;
	}
}
