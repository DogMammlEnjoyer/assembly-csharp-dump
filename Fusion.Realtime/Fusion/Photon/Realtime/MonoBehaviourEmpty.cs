using System;
using System.Collections;
using UnityEngine;

namespace Fusion.Photon.Realtime
{
	internal class MonoBehaviourEmpty : MonoBehaviour
	{
		public static MonoBehaviourEmpty BuildInstance(string id = null)
		{
			GameObject gameObject = new GameObject(id ?? "MonoBehaviourEmpty");
			Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<MonoBehaviourEmpty>();
		}

		public void SelfDestroy()
		{
			Object.Destroy(base.gameObject);
		}

		private void Update()
		{
			bool flag = this.obj != null;
			if (flag)
			{
				this.onCompleteCall(this.obj);
				this.obj = null;
				this.onCompleteCall = null;
				this.SelfDestroy();
			}
		}

		public void CompleteOnMainThread(RegionHandler obj)
		{
			this.obj = obj;
		}

		public void StartCoroutineAndDestroy(IEnumerator coroutine)
		{
			MonoBehaviourEmpty.<>c__DisplayClass6_0 CS$<>8__locals1 = new MonoBehaviourEmpty.<>c__DisplayClass6_0();
			CS$<>8__locals1.coroutine = coroutine;
			CS$<>8__locals1.<>4__this = this;
			base.StartCoroutine(CS$<>8__locals1.<StartCoroutineAndDestroy>g__Routine|0());
		}

		internal Action<RegionHandler> onCompleteCall;

		private RegionHandler obj;
	}
}
