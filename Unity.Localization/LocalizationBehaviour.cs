using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.Localization
{
	internal class LocalizationBehaviour : ComponentSingleton<LocalizationBehaviour>
	{
		protected override string GetGameObjectName()
		{
			return "Localization Resource Manager";
		}

		public static void ReleaseNextFrame(AsyncOperationHandle handle)
		{
			ComponentSingleton<LocalizationBehaviour>.Instance.DoReleaseNextFrame(handle);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long TimeSinceStartupMs()
		{
			return (long)(Time.realtimeSinceStartup * 1000f);
		}

		private void DoReleaseNextFrame(AsyncOperationHandle handle)
		{
			base.enabled = true;
			this.m_ReleaseQueue.Enqueue(new ValueTuple<int, AsyncOperationHandle>(Time.frameCount, handle));
		}

		private void LateUpdate()
		{
			int frameCount = Time.frameCount;
			long num = LocalizationBehaviour.TimeSinceStartupMs() + 10L;
			while (this.m_ReleaseQueue.Count > 0 && this.m_ReleaseQueue.Peek().Item1 < frameCount && LocalizationBehaviour.TimeSinceStartupMs() < num)
			{
				AddressablesInterface.SafeRelease(this.m_ReleaseQueue.Dequeue().Item2);
			}
			if (this.m_ReleaseQueue.Count == 0)
			{
				base.enabled = false;
			}
		}

		public static void ForceRelease()
		{
			foreach (ValueTuple<int, AsyncOperationHandle> valueTuple in ComponentSingleton<LocalizationBehaviour>.Instance.m_ReleaseQueue)
			{
				AddressablesInterface.SafeRelease(valueTuple.Item2);
			}
			ComponentSingleton<LocalizationBehaviour>.Instance.m_ReleaseQueue.Clear();
		}

		[TupleElementNames(new string[]
		{
			"frame",
			"handle"
		})]
		private Queue<ValueTuple<int, AsyncOperationHandle>> m_ReleaseQueue = new Queue<ValueTuple<int, AsyncOperationHandle>>();

		private const long k_MaxMsPerUpdate = 10L;

		private const bool k_DisableThrottling = false;
	}
}
