using System;
using System.Diagnostics;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Internal
{
	internal sealed class PlayerLoopRunner
	{
		public PlayerLoopRunner(PlayerLoopTiming timing)
		{
			this.unhandledExceptionCallback = delegate(Exception ex)
			{
				Debug.LogException(ex);
			};
			this.timing = timing;
		}

		public void AddAction(IPlayerLoopItem item)
		{
			object obj = this.runningAndQueueLock;
			lock (obj)
			{
				if (this.running)
				{
					this.waitQueue.Enqueue(item);
					return;
				}
			}
			obj = this.arrayLock;
			lock (obj)
			{
				if (this.loopItems.Length == this.tail)
				{
					Array.Resize<IPlayerLoopItem>(ref this.loopItems, checked(this.tail * 2));
				}
				IPlayerLoopItem[] array = this.loopItems;
				int num = this.tail;
				this.tail = num + 1;
				array[num] = item;
			}
		}

		public int Clear()
		{
			object obj = this.arrayLock;
			int result;
			lock (obj)
			{
				int num = 0;
				for (int i = 0; i < this.loopItems.Length; i++)
				{
					if (this.loopItems[i] != null)
					{
						num++;
					}
					this.loopItems[i] = null;
				}
				this.tail = 0;
				result = num;
			}
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
			object obj = this.runningAndQueueLock;
			lock (obj)
			{
				this.running = true;
			}
			obj = this.arrayLock;
			lock (obj)
			{
				int num = this.tail - 1;
				int i = 0;
				while (i < this.loopItems.Length)
				{
					IPlayerLoopItem playerLoopItem = this.loopItems[i];
					if (playerLoopItem != null)
					{
						try
						{
							if (!playerLoopItem.MoveNext())
							{
								this.loopItems[i] = null;
								goto IL_F9;
							}
							goto IL_106;
						}
						catch (Exception obj2)
						{
							this.loopItems[i] = null;
							try
							{
								this.unhandledExceptionCallback(obj2);
							}
							catch
							{
							}
							goto IL_F9;
						}
						goto IL_93;
					}
					goto IL_F9;
					IL_106:
					i++;
					continue;
					IL_93:
					IPlayerLoopItem playerLoopItem2 = this.loopItems[num];
					if (playerLoopItem2 != null)
					{
						try
						{
							if (!playerLoopItem2.MoveNext())
							{
								this.loopItems[num] = null;
								num--;
								goto IL_F9;
							}
							this.loopItems[i] = playerLoopItem2;
							this.loopItems[num] = null;
							num--;
							goto IL_106;
						}
						catch (Exception obj3)
						{
							this.loopItems[num] = null;
							num--;
							try
							{
								this.unhandledExceptionCallback(obj3);
							}
							catch
							{
							}
							goto IL_F9;
						}
					}
					num--;
					IL_F9:
					if (i >= num)
					{
						this.tail = i;
						break;
					}
					goto IL_93;
				}
				object obj4 = this.runningAndQueueLock;
				lock (obj4)
				{
					this.running = false;
					while (this.waitQueue.Count != 0)
					{
						if (this.loopItems.Length == this.tail)
						{
							Array.Resize<IPlayerLoopItem>(ref this.loopItems, checked(this.tail * 2));
						}
						IPlayerLoopItem[] array = this.loopItems;
						int num2 = this.tail;
						this.tail = num2 + 1;
						array[num2] = this.waitQueue.Dequeue();
					}
				}
			}
		}

		private const int InitialSize = 16;

		private readonly PlayerLoopTiming timing;

		private readonly object runningAndQueueLock = new object();

		private readonly object arrayLock = new object();

		private readonly Action<Exception> unhandledExceptionCallback;

		private int tail;

		private bool running;

		private IPlayerLoopItem[] loopItems = new IPlayerLoopItem[16];

		private MinimumQueue<IPlayerLoopItem> waitQueue = new MinimumQueue<IPlayerLoopItem>(16);
	}
}
