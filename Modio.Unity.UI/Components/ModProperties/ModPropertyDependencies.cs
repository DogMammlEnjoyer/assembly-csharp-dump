using System;
using Modio.Mods;
using Modio.Unity.UI.Search;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public class ModPropertyDependencies : IModProperty
	{
		public void OnModUpdate(Mod mod)
		{
			if (this._disableIfNoDependencies != null)
			{
				this._disableIfNoDependencies.SetActive(mod.Dependencies.HasDependencies);
			}
			if (this._dependenciesCount != null)
			{
				this._dependenciesCount.text = mod.Dependencies.Count.ToString();
			}
			if (this._searchDependencies != null)
			{
				this._searchDependencies.SetSearchForDependencies(mod);
			}
		}

		[SerializeField]
		private GameObject _disableIfNoDependencies;

		[SerializeField]
		private TMP_Text _dependenciesCount;

		[SerializeField]
		private ModioUISearch _searchDependencies;
	}
}
