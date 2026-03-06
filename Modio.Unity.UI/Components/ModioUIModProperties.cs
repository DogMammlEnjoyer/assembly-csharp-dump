using System;
using Modio.Unity.UI.Components.ModProperties;
using UnityEngine;

namespace Modio.Unity.UI.Components
{
	public class ModioUIModProperties : ModioUIPropertiesBase<ModioUIMod, IModProperty>
	{
		protected override IModProperty[] Properties
		{
			get
			{
				return this._properties;
			}
		}

		protected override void UpdateProperties()
		{
			if (this.Owner.Mod == null)
			{
				return;
			}
			IModProperty[] properties = this._properties;
			for (int i = 0; i < properties.Length; i++)
			{
				properties[i].OnModUpdate(this.Owner.Mod);
			}
		}

		[SerializeReference]
		private IModProperty[] _properties = Array.Empty<IModProperty>();
	}
}
