using System;
using Backtrace.Unity.Model.Waiter;
using UnityEngine;

namespace Backtrace.Unity.Model
{
	public class WaitForFrame
	{
		public static YieldInstruction Wait()
		{
			return WaitForFrame._waiter.Wait();
		}

		private static IWaiter CreateWaiterStrategy()
		{
			if (Application.isBatchMode)
			{
				return new BatchModeWaiter();
			}
			return new EndOfFrameWaiter();
		}

		private static IWaiter _waiter = WaitForFrame.CreateWaiterStrategy();
	}
}
