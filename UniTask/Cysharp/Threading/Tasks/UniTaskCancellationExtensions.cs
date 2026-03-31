using System;
using System.Threading;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace Cysharp.Threading.Tasks
{
	public static class UniTaskCancellationExtensions
	{
		public static CancellationToken GetCancellationTokenOnDestroy(this MonoBehaviour monoBehaviour)
		{
			return monoBehaviour.destroyCancellationToken;
		}

		public static CancellationToken GetCancellationTokenOnDestroy(this GameObject gameObject)
		{
			return gameObject.GetAsyncDestroyTrigger().CancellationToken;
		}

		public static CancellationToken GetCancellationTokenOnDestroy(this Component component)
		{
			MonoBehaviour monoBehaviour = component as MonoBehaviour;
			if (monoBehaviour != null)
			{
				return monoBehaviour.destroyCancellationToken;
			}
			return component.GetAsyncDestroyTrigger().CancellationToken;
		}
	}
}
