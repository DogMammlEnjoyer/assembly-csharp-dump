using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.ResourceManagement.AsyncOperations
{
	internal class GetDownloadSizeOperation : AsyncOperationBase<long>
	{
		public void Init(IEnumerable<IResourceLocation> locations, ResourceManager resourceManager)
		{
			this.m_Locations = locations;
			this.m_RM = resourceManager;
		}

		private IEnumerator Calculate()
		{
			long size = 0L;
			foreach (IResourceLocation resourceLocation in this.m_Locations)
			{
				ILocationSizeData locationSizeData = resourceLocation.Data as ILocationSizeData;
				if (locationSizeData != null)
				{
					size += locationSizeData.ComputeSize(resourceLocation, this.m_RM);
					yield return null;
				}
			}
			IEnumerator<IResourceLocation> enumerator = null;
			base.Complete(size, true, "");
			yield break;
			yield break;
		}

		private void CalculateSync()
		{
			long num = 0L;
			foreach (IResourceLocation resourceLocation in this.m_Locations)
			{
				ILocationSizeData locationSizeData = resourceLocation.Data as ILocationSizeData;
				if (locationSizeData != null)
				{
					num += locationSizeData.ComputeSize(resourceLocation, this.m_RM);
				}
			}
			base.Complete(num, true, "");
		}

		protected override void Execute()
		{
			this.m_AsyncCalculation = ComponentSingleton<MonoBehaviourCallbackHooks>.Instance.StartCoroutine(this.Calculate());
		}

		protected override bool InvokeWaitForCompletion()
		{
			ComponentSingleton<MonoBehaviourCallbackHooks>.Instance.StopCoroutine(this.m_AsyncCalculation);
			this.CalculateSync();
			return true;
		}

		private IEnumerable<IResourceLocation> m_Locations;

		private Coroutine m_AsyncCalculation;
	}
}
