using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Meta.Net.NativeWebSocket
{
	public class WaitForUpdate : CustomYieldInstruction
	{
		public override bool keepWaiting
		{
			get
			{
				return false;
			}
		}

		public WaitForUpdate.MainThreadAwaiter GetAwaiter()
		{
			WaitForUpdate.MainThreadAwaiter mainThreadAwaiter = new WaitForUpdate.MainThreadAwaiter();
			MainThreadUtil.Run(WaitForUpdate.CoroutineWrapper(this, mainThreadAwaiter));
			return mainThreadAwaiter;
		}

		public static IEnumerator CoroutineWrapper(IEnumerator theWorker, WaitForUpdate.MainThreadAwaiter awaiter)
		{
			yield return theWorker;
			awaiter.Complete();
			yield break;
		}

		public class MainThreadAwaiter : INotifyCompletion
		{
			public bool IsCompleted { get; set; }

			void INotifyCompletion.OnCompleted(Action continuation)
			{
				this.continuation = continuation;
			}

			public void GetResult()
			{
			}

			public void Complete()
			{
				this.IsCompleted = true;
				Action action = this.continuation;
				if (action == null)
				{
					return;
				}
				action();
			}

			private Action continuation;
		}
	}
}
