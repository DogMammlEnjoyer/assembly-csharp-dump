using System;
using Modio.Users;
using UnityEngine;
using UnityEngine.Events;

namespace Modio.Unity.UI.Components
{
	public class ModioUIUser : MonoBehaviour, IModioUIPropertiesOwner
	{
		public UserProfile User { get; private set; }

		private void Start()
		{
			if (this._useLoggedInUser)
			{
				Modio.Users.User.OnUserChanged += this.OnUserChanged;
				this.GetCurrentUser();
			}
		}

		private void OnDestroy()
		{
			if (this.User != null)
			{
				this.User.OnProfileUpdated -= this.ProfileUpdated;
			}
			Modio.Users.User.OnUserChanged -= this.OnUserChanged;
		}

		public void AddUpdatePropertiesListener(UnityAction listener)
		{
			this.onUserUpdate.AddListener(listener);
		}

		public void RemoveUpdatePropertiesListener(UnityAction listener)
		{
			this.onUserUpdate.RemoveListener(listener);
		}

		private void GetCurrentUser()
		{
			User user = Modio.Users.User.Current;
			this.SetUser((user != null) ? user.Profile : null);
		}

		private void OnUserChanged(User user)
		{
			this.SetUser(user.Profile);
		}

		public void SetUser(UserProfile profile)
		{
			if (this.User != null)
			{
				this.User.OnProfileUpdated -= this.ProfileUpdated;
			}
			this.User = profile;
			if (profile != null)
			{
				this.User.OnProfileUpdated += this.ProfileUpdated;
			}
			this.ProfileUpdated();
		}

		private void ProfileUpdated()
		{
			this.onUserUpdate.Invoke();
		}

		public UnityEvent onUserUpdate;

		[SerializeField]
		private bool _useLoggedInUser;
	}
}
