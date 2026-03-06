using System;
using UnityEngine;

namespace Backtrace.Unity.Model.Waiter
{
	public interface IWaiter
	{
		YieldInstruction Wait();
	}
}
