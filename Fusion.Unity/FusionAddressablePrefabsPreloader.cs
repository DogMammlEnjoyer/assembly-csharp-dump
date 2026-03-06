using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Fusion
{
	public class FusionAddressablePrefabsPreloader : MonoBehaviour
	{
		private void Start()
		{
			FusionAddressablePrefabsPreloader.<Start>d__1 <Start>d__;
			<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Start>d__.<>4__this = this;
			<Start>d__.<>1__state = -1;
			<Start>d__.<>t__builder.Start<FusionAddressablePrefabsPreloader.<Start>d__1>(ref <Start>d__);
		}

		private void OnDestroy()
		{
			foreach (AsyncOperationHandle<GameObject> handle in this._handles)
			{
				Addressables.Release<GameObject>(handle);
			}
		}

		private List<AsyncOperationHandle<GameObject>> _handles = new List<AsyncOperationHandle<GameObject>>();
	}
}
