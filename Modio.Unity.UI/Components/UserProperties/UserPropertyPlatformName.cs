using System;
using Modio.API;
using Modio.Users;
using UnityEngine;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.UserProperties
{
	[Serializable]
	public class UserPropertyPlatformName : IUserProperty
	{
		public void OnUserUpdate(UserProfile user)
		{
			bool flag = user != null && !string.IsNullOrEmpty(user.PortalUsername);
			GameObject[] enableIsUsernameDefinedByPortal = this._enableIsUsernameDefinedByPortal;
			for (int i = 0; i < enableIsUsernameDefinedByPortal.Length; i++)
			{
				enableIsUsernameDefinedByPortal[i].SetActive(flag);
			}
			if (this._platformImage != null && flag)
			{
				Sprite sprite = null;
				ModioAPI.Portal currentPortal = ModioAPI.CurrentPortal;
				foreach (UserPropertyPlatformName.PlatformIcon platformIcon in this._platformIcons)
				{
					if (platformIcon.Portal == currentPortal)
					{
						sprite = platformIcon.Icon;
					}
				}
				this._platformImage.enabled = (sprite != null);
				this._platformImage.sprite = sprite;
			}
		}

		[SerializeField]
		private GameObject[] _enableIsUsernameDefinedByPortal;

		[SerializeField]
		private Image _platformImage;

		[SerializeField]
		private UserPropertyPlatformName.PlatformIcon[] _platformIcons;

		[Serializable]
		private class PlatformIcon
		{
			public ModioAPI.Portal Portal;

			public Sprite Icon;
		}
	}
}
