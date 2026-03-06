using System;
using Modio.Mods;
using UnityEngine;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public class ModPropertySubscribed : IModProperty
	{
		public void OnModUpdate(Mod mod)
		{
			if (this._notSubscribedActive != null)
			{
				this._notSubscribedActive.SetActive(!mod.IsSubscribed);
			}
			if (this._subscribedActive != null)
			{
				this._subscribedActive.SetActive(mod.IsSubscribed);
			}
		}

		[SerializeField]
		private GameObject _notSubscribedActive;

		[SerializeField]
		private GameObject _subscribedActive;
	}
}
