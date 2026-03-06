using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
	public class InstanceProvider : IInstanceProvider
	{
		public GameObject ProvideInstance(ResourceManager resourceManager, AsyncOperationHandle<GameObject> prefabHandle, InstantiationParameters instantiateParameters)
		{
			GameObject gameObject = instantiateParameters.Instantiate<GameObject>(prefabHandle.Result);
			this.m_InstanceObjectToPrefabHandle.Add(gameObject, prefabHandle);
			return gameObject;
		}

		public void ReleaseInstance(ResourceManager resourceManager, GameObject instance)
		{
			if (instance == null)
			{
				return;
			}
			AsyncOperationHandle<GameObject> asyncOperationHandle;
			if (!this.m_InstanceObjectToPrefabHandle.TryGetValue(instance, out asyncOperationHandle))
			{
				Debug.LogWarningFormat("Releasing unknown GameObject {0} to InstanceProvider.", new object[]
				{
					instance
				});
			}
			else
			{
				asyncOperationHandle.Release();
				this.m_InstanceObjectToPrefabHandle.Remove(instance);
			}
			if (instance != null)
			{
				if (Application.isPlaying)
				{
					Object.Destroy(instance);
					return;
				}
				Object.DestroyImmediate(instance);
			}
		}

		private Dictionary<GameObject, AsyncOperationHandle<GameObject>> m_InstanceObjectToPrefabHandle = new Dictionary<GameObject, AsyncOperationHandle<GameObject>>();
	}
}
