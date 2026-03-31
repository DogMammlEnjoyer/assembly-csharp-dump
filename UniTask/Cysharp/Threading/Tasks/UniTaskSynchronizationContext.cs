using System;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks
{
	public class UniTaskSynchronizationContext : SynchronizationContext
	{
		public override void Send(SendOrPostCallback d, object state)
		{
			d(state);
		}

		public override void Post(SendOrPostCallback d, object state)
		{
			bool flag = false;
			try
			{
				UniTaskSynchronizationContext.gate.Enter(ref flag);
				if (UniTaskSynchronizationContext.dequing)
				{
					if (UniTaskSynchronizationContext.waitingList.Length == UniTaskSynchronizationContext.waitingListCount)
					{
						int num = UniTaskSynchronizationContext.waitingListCount * 2;
						if (num > 2146435071)
						{
							num = 2146435071;
						}
						UniTaskSynchronizationContext.Callback[] destinationArray = new UniTaskSynchronizationContext.Callback[num];
						Array.Copy(UniTaskSynchronizationContext.waitingList, destinationArray, UniTaskSynchronizationContext.waitingListCount);
						UniTaskSynchronizationContext.waitingList = destinationArray;
					}
					UniTaskSynchronizationContext.waitingList[UniTaskSynchronizationContext.waitingListCount] = new UniTaskSynchronizationContext.Callback(d, state);
					UniTaskSynchronizationContext.waitingListCount++;
				}
				else
				{
					if (UniTaskSynchronizationContext.actionList.Length == UniTaskSynchronizationContext.actionListCount)
					{
						int num2 = UniTaskSynchronizationContext.actionListCount * 2;
						if (num2 > 2146435071)
						{
							num2 = 2146435071;
						}
						UniTaskSynchronizationContext.Callback[] destinationArray2 = new UniTaskSynchronizationContext.Callback[num2];
						Array.Copy(UniTaskSynchronizationContext.actionList, destinationArray2, UniTaskSynchronizationContext.actionListCount);
						UniTaskSynchronizationContext.actionList = destinationArray2;
					}
					UniTaskSynchronizationContext.actionList[UniTaskSynchronizationContext.actionListCount] = new UniTaskSynchronizationContext.Callback(d, state);
					UniTaskSynchronizationContext.actionListCount++;
				}
			}
			finally
			{
				if (flag)
				{
					UniTaskSynchronizationContext.gate.Exit(false);
				}
			}
		}

		public override void OperationStarted()
		{
			Interlocked.Increment(ref UniTaskSynchronizationContext.opCount);
		}

		public override void OperationCompleted()
		{
			Interlocked.Decrement(ref UniTaskSynchronizationContext.opCount);
		}

		public override SynchronizationContext CreateCopy()
		{
			return this;
		}

		internal static void Run()
		{
			bool flag = false;
			try
			{
				UniTaskSynchronizationContext.gate.Enter(ref flag);
				if (UniTaskSynchronizationContext.actionListCount == 0)
				{
					return;
				}
				UniTaskSynchronizationContext.dequing = true;
			}
			finally
			{
				if (flag)
				{
					UniTaskSynchronizationContext.gate.Exit(false);
				}
			}
			for (int i = 0; i < UniTaskSynchronizationContext.actionListCount; i++)
			{
				UniTaskSynchronizationContext.Callback callback = UniTaskSynchronizationContext.actionList[i];
				UniTaskSynchronizationContext.actionList[i] = default(UniTaskSynchronizationContext.Callback);
				callback.Invoke();
			}
			bool flag2 = false;
			try
			{
				UniTaskSynchronizationContext.gate.Enter(ref flag2);
				UniTaskSynchronizationContext.dequing = false;
				UniTaskSynchronizationContext.Callback[] array = UniTaskSynchronizationContext.actionList;
				UniTaskSynchronizationContext.actionListCount = UniTaskSynchronizationContext.waitingListCount;
				UniTaskSynchronizationContext.actionList = UniTaskSynchronizationContext.waitingList;
				UniTaskSynchronizationContext.waitingListCount = 0;
				UniTaskSynchronizationContext.waitingList = array;
			}
			finally
			{
				if (flag2)
				{
					UniTaskSynchronizationContext.gate.Exit(false);
				}
			}
		}

		private const int MaxArrayLength = 2146435071;

		private const int InitialSize = 16;

		private static SpinLock gate = new SpinLock(false);

		private static bool dequing = false;

		private static int actionListCount = 0;

		private static UniTaskSynchronizationContext.Callback[] actionList = new UniTaskSynchronizationContext.Callback[16];

		private static int waitingListCount = 0;

		private static UniTaskSynchronizationContext.Callback[] waitingList = new UniTaskSynchronizationContext.Callback[16];

		private static int opCount;

		[StructLayout(LayoutKind.Auto)]
		private readonly struct Callback
		{
			public Callback(SendOrPostCallback callback, object state)
			{
				this.callback = callback;
				this.state = state;
			}

			public void Invoke()
			{
				try
				{
					this.callback(this.state);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}

			private readonly SendOrPostCallback callback;

			private readonly object state;
		}
	}
}
