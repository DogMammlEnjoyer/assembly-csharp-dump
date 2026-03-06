using System;
using Modio.Mods;
using UnityEngine;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public class ModPropertyDateUpdated : ModPropertyDateBase
	{
		protected override DateTime GetValue(Mod mod)
		{
			return mod.DateUpdated;
		}

		public override void OnModUpdate(Mod mod)
		{
			base.OnModUpdate(mod);
			if (this._disableIfNoUpdate != null)
			{
				this._disableIfNoUpdate.SetActive(mod.DateUpdated != mod.DateLive);
			}
		}

		[SerializeField]
		private GameObject _disableIfNoUpdate;
	}
}
