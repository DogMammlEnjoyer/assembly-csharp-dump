using System;
using System.Collections.Generic;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaNetworking
{
	public class CosmeticItemRegistry
	{
		public VRRig Rig
		{
			get
			{
				return this.rig;
			}
		}

		public void RefreshRig()
		{
			this.rig.RefreshCosmetics();
		}

		public CosmeticItemRegistry(VRRig _rig)
		{
			this.rig = _rig;
		}

		public void InitializeCosmetic(GameObject cosmeticGObj, bool isOverride)
		{
			if (this.initializedCosmetics.Contains(cosmeticGObj))
			{
				return;
			}
			this.initializedCosmetics.Add(cosmeticGObj);
			if (!isOverride)
			{
				foreach (GameObject gameObject in this.rig.overrideCosmetics)
				{
					if (cosmeticGObj.name == gameObject.name)
					{
						cosmeticGObj.name = "OVERRIDDEN";
						return;
					}
				}
			}
			string text = cosmeticGObj.name.Replace("LEFT.", "").Replace("RIGHT.", "").TrimEnd();
			CosmeticItemInstance cosmeticItemInstance;
			if (this._nameToCosmeticMap.ContainsKey(text))
			{
				cosmeticItemInstance = this._nameToCosmeticMap[text];
			}
			else
			{
				cosmeticItemInstance = new CosmeticItemInstance();
				CosmeticSO cosmeticSOFromDisplayName = CosmeticsController.instance.GetCosmeticSOFromDisplayName(text);
				cosmeticItemInstance.clippingOffsets = ((cosmeticSOFromDisplayName != null) ? cosmeticSOFromDisplayName.info.anchorAntiIntersectOffsets : CosmeticsController.instance.defaultClipOffsets);
				cosmeticItemInstance.isHoldableItem = (cosmeticSOFromDisplayName != null && cosmeticSOFromDisplayName.info.hasHoldableParts);
				this._nameToCosmeticMap.Add(text, cosmeticItemInstance);
			}
			HoldableObject component = cosmeticGObj.GetComponent<HoldableObject>();
			bool flag = cosmeticGObj.name.Contains("LEFT.");
			bool flag2 = cosmeticGObj.name.Contains("RIGHT.");
			if (cosmeticItemInstance.isHoldableItem && component != null)
			{
				if (component is SnowballThrowable || component is TransferrableObject)
				{
					cosmeticItemInstance.holdableObjects.Add(cosmeticGObj);
				}
				else if (flag)
				{
					cosmeticItemInstance.leftObjects.Add(cosmeticGObj);
				}
				else if (flag2)
				{
					cosmeticItemInstance.rightObjects.Add(cosmeticGObj);
				}
				else
				{
					cosmeticItemInstance.objects.Add(cosmeticGObj);
				}
			}
			else if (flag)
			{
				cosmeticItemInstance.leftObjects.Add(cosmeticGObj);
			}
			else if (flag2)
			{
				cosmeticItemInstance.rightObjects.Add(cosmeticGObj);
			}
			else
			{
				cosmeticItemInstance.objects.Add(cosmeticGObj);
			}
			cosmeticItemInstance.dbgname = text;
			Renderer[] componentsInChildren = cosmeticGObj.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].enabled)
				{
					cosmeticItemInstance.allRenderers.Add(componentsInChildren[i]);
				}
			}
			ParticleSystem[] componentsInChildren2 = cosmeticGObj.GetComponentsInChildren<ParticleSystem>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				if (componentsInChildren2[j].emission.enabled)
				{
					cosmeticItemInstance.allParticles.Add(componentsInChildren2[j]);
				}
			}
		}

		public CosmeticItemInstance Cosmetic(string playfabId)
		{
			if (string.IsNullOrEmpty(playfabId) || playfabId == "NOTHING")
			{
				return null;
			}
			CosmeticItemInstance result;
			if (!this._nameToCosmeticMap.TryGetValue(playfabId, out result))
			{
				CosmeticsV2Spawner_Dirty.ProcessLoadOpInfos(this.rig, playfabId, this);
				return null;
			}
			return result;
		}

		public void RequestCosmetic(string playfabId, ICosmeticRequestCallback callback)
		{
			if (!CosmeticsV2Spawner_Dirty.isPrepared && !CosmeticsV2Spawner_Dirty.isFinalizingSetup)
			{
				Debug.LogError("[GT/CosmeticItemRegistry]  ERROR!!!  RequestCosmetic: Cannot request cosmetic before cosmetic spawner is prepared.");
				return;
			}
			if (string.IsNullOrEmpty(playfabId) || playfabId == "NOTHING")
			{
				if (callback != null)
				{
					callback.OnCosmeticLoaded(playfabId, null);
				}
				return;
			}
			CosmeticItemInstance instance;
			if (this._nameToCosmeticMap.TryGetValue(playfabId, out instance))
			{
				if (callback != null)
				{
					callback.OnCosmeticLoaded(playfabId, instance);
				}
				return;
			}
			List<ICosmeticRequestCallback> list;
			if (!this._pendingCallbacks.TryGetValue(playfabId, out list))
			{
				list = new List<ICosmeticRequestCallback>(4);
				this._pendingCallbacks.Add(playfabId, list);
			}
			list.Add(callback);
			CosmeticsV2Spawner_Dirty.ProcessLoadOpInfos(this.rig, playfabId, this);
		}

		public Awaitable<CosmeticItemInstance> AwaitCosmetic(string playfabId)
		{
			AwaitableCompletionSource<CosmeticItemInstance> awaitableCompletionSource = new AwaitableCompletionSource<CosmeticItemInstance>();
			if (!CosmeticsV2Spawner_Dirty.isPrepared && !CosmeticsV2Spawner_Dirty.isFinalizingSetup)
			{
				Debug.LogError("[GT/CosmeticItemRegistry]  ERROR!!!  AwaitCosmetic: Cannot request cosmetic before cosmetic spawner is prepared.");
				AwaitableCompletionSource<CosmeticItemInstance> awaitableCompletionSource2 = awaitableCompletionSource;
				CosmeticItemInstance cosmeticItemInstance = null;
				awaitableCompletionSource2.SetResult(cosmeticItemInstance);
				return awaitableCompletionSource.Awaitable;
			}
			this.RequestCosmetic(playfabId, new CosmeticItemRegistry._AwaitCosmeticRequestCallback(awaitableCompletionSource));
			return awaitableCompletionSource.Awaitable;
		}

		public void FlushPendingCallbacks()
		{
			if (this._pendingCallbacks.Count == 0)
			{
				return;
			}
			CosmeticItemRegistry._flushKeysBuffer.Clear();
			foreach (KeyValuePair<string, List<ICosmeticRequestCallback>> keyValuePair in this._pendingCallbacks)
			{
				string text;
				List<ICosmeticRequestCallback> list;
				keyValuePair.Deconstruct(out text, out list);
				string text2 = text;
				List<ICosmeticRequestCallback> list2 = list;
				CosmeticItemInstance instance;
				if (this._nameToCosmeticMap.TryGetValue(text2, out instance))
				{
					for (int i = 0; i < list2.Count; i++)
					{
						if (list2[i] != null)
						{
							list2[i].OnCosmeticLoaded(text2, instance);
						}
					}
					list2.Clear();
					CosmeticItemRegistry._flushKeysBuffer.Add(text2);
				}
			}
			for (int j = 0; j < CosmeticItemRegistry._flushKeysBuffer.Count; j++)
			{
				this._pendingCallbacks.Remove(CosmeticItemRegistry._flushKeysBuffer[j]);
			}
		}

		private Dictionary<string, CosmeticItemInstance> _nameToCosmeticMap = new Dictionary<string, CosmeticItemInstance>();

		private HashSet<GameObject> initializedCosmetics = new HashSet<GameObject>();

		private readonly Dictionary<string, List<ICosmeticRequestCallback>> _pendingCallbacks = new Dictionary<string, List<ICosmeticRequestCallback>>();

		private GameObject _nullItem;

		private VRRig rig;

		private static readonly List<string> _flushKeysBuffer = new List<string>(32);

		private struct _AwaitCosmeticRequestCallback : ICosmeticRequestCallback
		{
			public _AwaitCosmeticRequestCallback(AwaitableCompletionSource<CosmeticItemInstance> source)
			{
				this._source = source;
			}

			public void OnCosmeticLoaded(string itemName, CosmeticItemInstance instance)
			{
				this._source.SetResult(instance);
			}

			private AwaitableCompletionSource<CosmeticItemInstance> _source;
		}
	}
}
