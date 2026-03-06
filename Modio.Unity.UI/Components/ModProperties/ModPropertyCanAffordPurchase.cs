using System;
using Modio.Mods;
using Modio.Users;
using UnityEngine;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public class ModPropertyCanAffordPurchase : IModProperty
	{
		public void OnModUpdate(Mod mod)
		{
			User user = User.Current;
			long num = (user != null) ? user.Wallet.Balance : 0L;
			bool flag = mod.Price <= num;
			GameObject[] array = this._activateWhenCanAfford;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(flag);
			}
			array = this._activateWhenCanNotAfford;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(!flag);
			}
		}

		[SerializeField]
		private GameObject[] _activateWhenCanAfford;

		[SerializeField]
		private GameObject[] _activateWhenCanNotAfford;
	}
}
