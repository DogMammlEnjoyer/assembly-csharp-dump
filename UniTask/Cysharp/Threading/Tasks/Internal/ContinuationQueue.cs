using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Internal
{
	internal sealed class ContinuationQueue
	{
		public ContinuationQueue(PlayerLoopTiming timing)
		{
			this.timing = timing;
		}

		public void Enqueue(Action continuation)
		{
			bool flag = false;
			try
			{
				this.gate.Enter(ref flag);
				if (this.dequing)
				{
					if (this.waitingList.Length == this.waitingListCount)
					{
						int num = this.waitingListCount * 2;
						if (num > 2146435071)
						{
							num = 2146435071;
						}
						Action[] destinationArray = new Action[num];
						Array.Copy(this.waitingList, destinationArray, this.waitingListCount);
						this.waitingList = destinationArray;
					}
					this.waitingList[this.waitingListCount] = continuation;
					this.waitingListCount++;
				}
				else
				{
					if (this.actionList.Length == this.actionListCount)
					{
						int num2 = this.actionListCount * 2;
						if (num2 > 2146435071)
						{
							num2 = 2146435071;
						}
						Action[] destinationArray2 = new Action[num2];
						Array.Copy(this.actionList, destinationArray2, this.actionListCount);
						this.actionList = destinationArray2;
					}
					this.actionList[this.actionListCount] = continuation;
					this.actionListCount++;
				}
			}
			finally
			{
				if (flag)
				{
					this.gate.Exit(false);
				}
			}
		}

		public int Clear()
		{
			int result = this.actionListCount + this.waitingListCount;
			this.actionListCount = 0;
			this.actionList = new Action[16];
			this.waitingListCount = 0;
			this.waitingList = new Action[16];
			return result;
		}

		public void Run()
		{
			this.RunCore();
		}

		private void Initialization()
		{
			this.RunCore();
		}

		private void LastInitialization()
		{
			this.RunCore();
		}

		private void EarlyUpdate()
		{
			this.RunCore();
		}

		private void LastEarlyUpdate()
		{
			this.RunCore();
		}

		private void FixedUpdate()
		{
			this.RunCore();
		}

		private void LastFixedUpdate()
		{
			this.RunCore();
		}

		private void PreUpdate()
		{
			this.RunCore();
		}

		private void LastPreUpdate()
		{
			this.RunCore();
		}

		private void Update()
		{
			this.RunCore();
		}

		private void LastUpdate()
		{
			this.RunCore();
		}

		private void PreLateUpdate()
		{
			this.RunCore();
		}

		private void LastPreLateUpdate()
		{
			this.RunCore();
		}

		private void PostLateUpdate()
		{
			this.RunCore();
		}

		private void LastPostLateUpdate()
		{
			this.RunCore();
		}

		private void TimeUpdate()
		{
			this.RunCore();
		}

		private void LastTimeUpdate()
		{
			this.RunCore();
		}

		[DebuggerHidden]
		private void RunCore()
		{
			bool flag = false;
			try
			{
				this.gate.Enter(ref flag);
				if (this.actionListCount == 0)
				{
					return;
				}
				this.dequing = true;
			}
			finally
			{
				if (flag)
				{
					this.gate.Exit(false);
				}
			}
			for (int i = 0; i < this.actionListCount; i++)
			{
				Action action = this.actionList[i];
				this.actionList[i] = null;
				try
				{
					action();
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
			bool flag2 = false;
			try
			{
				this.gate.Enter(ref flag2);
				this.dequing = false;
				Action[] array = this.actionList;
				this.actionListCount = this.waitingListCount;
				this.actionList = this.waitingList;
				this.waitingListCount = 0;
				this.waitingList = array;
			}
			finally
			{
				if (flag2)
				{
					this.gate.Exit(false);
				}
			}
		}

		private const int MaxArrayLength = 2146435071;

		private const int InitialSize = 16;

		private readonly PlayerLoopTiming timing;

		private SpinLock gate = new SpinLock(false);

		private bool dequing;

		private int actionListCount;

		private Action[] actionList = new Action[16];

		private int waitingListCount;

		private Action[] waitingList = new Action[16];
	}
}
