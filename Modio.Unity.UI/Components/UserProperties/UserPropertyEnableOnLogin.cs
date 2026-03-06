using System;
using Modio.Users;
using UnityEngine;

namespace Modio.Unity.UI.Components.UserProperties
{
	[Serializable]
	public class UserPropertyEnableOnLogin : IUserProperty
	{
		public void OnUserUpdate(UserProfile user)
		{
			bool flag = user != null;
			GameObject[] array = this._activeWhenLoggedOut;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(!flag);
			}
			array = this._activeWhenLoggedIn;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(flag);
			}
		}

		[SerializeField]
		private GameObject[] _activeWhenLoggedOut;

		[SerializeField]
		private GameObject[] _activeWhenLoggedIn;
	}
}
