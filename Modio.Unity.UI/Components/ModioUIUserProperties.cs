using System;
using Modio.Unity.UI.Components.UserProperties;
using UnityEngine;

namespace Modio.Unity.UI.Components
{
	public class ModioUIUserProperties : ModioUIPropertiesBase<ModioUIUser, IUserProperty>
	{
		protected override IUserProperty[] Properties
		{
			get
			{
				return this._properties;
			}
		}

		protected override void UpdateProperties()
		{
			IUserProperty[] properties = this._properties;
			for (int i = 0; i < properties.Length; i++)
			{
				properties[i].OnUserUpdate(this.Owner.User);
			}
		}

		[SerializeReference]
		private IUserProperty[] _properties = Array.Empty<IUserProperty>();
	}
}
