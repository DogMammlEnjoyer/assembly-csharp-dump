using System;
using Modio.Mods;
using UnityEngine;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public class ModPropertyPrice : ModPropertyNumberBase
	{
		protected override long GetValue(Mod mod)
		{
			if (this._disableIfFree != null)
			{
				this._disableIfFree.SetActive(mod.IsMonetized && (!this._alsoDisableIfPurchased || !mod.IsPurchased));
			}
			if (this._enableIfPurchased != null)
			{
				this._enableIfPurchased.SetActive(mod.IsPurchased);
			}
			return mod.Price;
		}

		[SerializeField]
		private GameObject _disableIfFree;

		[SerializeField]
		private bool _alsoDisableIfPurchased;

		[SerializeField]
		private GameObject _enableIfPurchased;
	}
}
