using System;
using DefaultNamespace;
using GorillaNetworking;
using GorillaNetworking.Store;
using UnityEngine;

namespace CosmeticRoom
{
	public class EvolvingCosmeticKioskButtonSet : MonoBehaviour
	{
		public void RegisterKiosk(EvolvingCosmeticKiosk kiosk)
		{
			if (this._kiosk != null)
			{
				throw new Exception("Attempted to double-register EvolvingCosmeticKiosk to a button.");
			}
			this._kiosk = kiosk;
		}

		public void Reset()
		{
			this._cosmeticStand.ClearCosmetics();
			this._playfabId = null;
			this._cosmetic = null;
		}

		public void SetCosmetic(string playfabId, EvolvingCosmetic evolvingCosmetic)
		{
			this._cosmeticStand.SpawnItemOntoStand(playfabId);
			this._playfabId = playfabId;
			this._cosmetic = evolvingCosmetic;
		}

		public void GoForward()
		{
			if (this._cosmetic == null || !this._cosmetic.CanGoForward())
			{
				return;
			}
			this._cosmetic.GoForward();
			this.RefreshOnPlayer();
		}

		public void GoBackward()
		{
			if (this._cosmetic == null || !this._cosmetic.CanGoBack())
			{
				return;
			}
			this._cosmetic.GoBack();
			this.RefreshOnPlayer();
		}

		private void RefreshOnPlayer()
		{
			if (this._kiosk == null || this._playfabId == null || this._cosmetic == null)
			{
				return;
			}
			bool flag = false;
			CosmeticsController.CosmeticItem[] items = CosmeticsController.instance.currentWornSet.items;
			for (int i = 0; i < items.Length; i++)
			{
				if (!(items[i].itemName != this._playfabId))
				{
					CosmeticItemInstance cosmeticItemInstance = this._kiosk.VRRig.cosmeticsObjectRegistry.Cosmetic(this._playfabId);
					if (cosmeticItemInstance != null)
					{
						foreach (GameObject gameObject in cosmeticItemInstance.objects)
						{
							EvolvingCosmetic component = gameObject.GetComponent<EvolvingCosmetic>();
							if (component != null)
							{
								component.MatchStage(this._cosmetic);
								EvolvingCosmeticSaveData.Instance.SelectedIndices[component.PlayfabId] = component.SelectedObjectIndex;
								flag = true;
							}
						}
					}
				}
			}
			if (flag)
			{
				PlayerPrefs.SetString("EvolvingCosmeticSaveData", EvolvingCosmeticSaveData.Instance.Write());
			}
		}

		[SerializeField]
		private DynamicCosmeticStand _cosmeticStand;

		[SerializeField]
		private GorillaPressableButton _plusButton;

		[SerializeField]
		private GorillaPressableButton _minusButton;

		private EvolvingCosmeticKiosk _kiosk;

		private EvolvingCosmetic _cosmetic;

		private string _playfabId;
	}
}
