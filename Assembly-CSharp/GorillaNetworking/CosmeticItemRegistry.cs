using System;
using System.Collections.Generic;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaNetworking
{
	public class CosmeticItemRegistry
	{
		public bool isInitialized
		{
			get
			{
				return this._isInitialized;
			}
		}

		public void Initialize(GameObject[] cosmeticGObjs)
		{
			if (this._isInitialized)
			{
				return;
			}
			this._isInitialized = true;
			foreach (GameObject gameObject in cosmeticGObjs)
			{
				string text = gameObject.name.Replace("LEFT.", "").Replace("RIGHT.", "").TrimEnd();
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
				HoldableObject component = gameObject.GetComponent<HoldableObject>();
				bool flag = gameObject.name.Contains("LEFT.");
				bool flag2 = gameObject.name.Contains("RIGHT.");
				if (cosmeticItemInstance.isHoldableItem && component != null)
				{
					if (component is SnowballThrowable || component is TransferrableObject)
					{
						cosmeticItemInstance.holdableObjects.Add(gameObject);
					}
					else if (flag)
					{
						cosmeticItemInstance.leftObjects.Add(gameObject);
					}
					else if (flag2)
					{
						cosmeticItemInstance.rightObjects.Add(gameObject);
					}
					else
					{
						cosmeticItemInstance.objects.Add(gameObject);
					}
				}
				else if (flag)
				{
					cosmeticItemInstance.leftObjects.Add(gameObject);
				}
				else if (flag2)
				{
					cosmeticItemInstance.rightObjects.Add(gameObject);
				}
				else
				{
					cosmeticItemInstance.objects.Add(gameObject);
				}
				cosmeticItemInstance.dbgname = text;
				Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					if (componentsInChildren[j].enabled)
					{
						cosmeticItemInstance.allRenderers.Add(componentsInChildren[j]);
					}
				}
				ParticleSystem[] componentsInChildren2 = gameObject.GetComponentsInChildren<ParticleSystem>();
				for (int k = 0; k < componentsInChildren2.Length; k++)
				{
					if (componentsInChildren2[k].emission.enabled)
					{
						cosmeticItemInstance.allParticles.Add(componentsInChildren2[k]);
					}
				}
			}
		}

		public CosmeticItemInstance Cosmetic(string itemName)
		{
			if (!this._isInitialized)
			{
				Debug.LogError("Tried to use CosmeticItemRegistry before it was initialized!");
				return null;
			}
			if (string.IsNullOrEmpty(itemName) || itemName == "NOTHING")
			{
				return null;
			}
			CosmeticItemInstance result;
			if (!this._nameToCosmeticMap.TryGetValue(itemName, out result))
			{
				return null;
			}
			return result;
		}

		private bool _isInitialized;

		private Dictionary<string, CosmeticItemInstance> _nameToCosmeticMap = new Dictionary<string, CosmeticItemInstance>();

		private GameObject _nullItem;
	}
}
