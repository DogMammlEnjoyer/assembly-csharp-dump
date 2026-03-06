using System;
using Modio.Mods;
using UnityEngine;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public class ModPropertyInstalled : IModProperty
	{
		public void OnModUpdate(Mod mod)
		{
			if (this._notInstalledActive != null)
			{
				this._notInstalledActive.SetActive(mod.File.State != ModFileState.Installed);
			}
			if (this._installedActive != null)
			{
				this._installedActive.SetActive(mod.File.State == ModFileState.Installed);
			}
		}

		[SerializeField]
		private GameObject _notInstalledActive;

		[SerializeField]
		private GameObject _installedActive;
	}
}
