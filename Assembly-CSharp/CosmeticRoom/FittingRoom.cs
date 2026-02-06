using System;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

namespace CosmeticRoom
{
	public class FittingRoom : MonoBehaviour
	{
		public void InitializeForCustomMap(bool useCustomConsoleMesh = true)
		{
			GameObject gameObject = this.consoleMesh;
			if (gameObject != null)
			{
				gameObject.SetActive(!useCustomConsoleMesh);
			}
			CosmeticsController.instance.AddFittingRoom(this);
		}

		private void OnEnable()
		{
			if (this.addOnEnable)
			{
				CosmeticsController.instance.AddFittingRoom(this);
			}
		}

		private void OnDisable()
		{
			if (this.addOnEnable)
			{
				CosmeticsController.instance.RemoveFittingRoom(this);
			}
		}

		public void UpdateFromCart(List<CosmeticsController.CosmeticItem> currentCart, CosmeticsController.CosmeticSet tryOnSet)
		{
			this.iterator = 0;
			while (this.iterator < this.fittingRoomButtons.Length)
			{
				if (this.iterator < currentCart.Count)
				{
					bool isInTryOnSet = CosmeticsController.instance.AnyMatch(tryOnSet, currentCart[this.iterator]);
					this.fittingRoomButtons[this.iterator].SetItem(currentCart[this.iterator], isInTryOnSet);
				}
				else
				{
					this.fittingRoomButtons[this.iterator].ClearItem();
				}
				this.iterator++;
			}
		}

		public FittingRoomButton[] fittingRoomButtons;

		public GameObject consoleMesh;

		private int iterator;

		public bool addOnEnable;
	}
}
