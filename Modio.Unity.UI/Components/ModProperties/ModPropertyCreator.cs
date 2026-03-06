using System;
using Modio.Mods;
using UnityEngine;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public class ModPropertyCreator : IModProperty
	{
		public void OnModUpdate(Mod mod)
		{
			this._user.SetUser(mod.Creator);
		}

		[SerializeField]
		private ModioUIUser _user;
	}
}
