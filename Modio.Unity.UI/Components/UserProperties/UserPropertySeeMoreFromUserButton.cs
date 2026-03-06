using System;
using Modio.Unity.UI.Panels;
using Modio.Unity.UI.Search;
using Modio.Users;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.UserProperties
{
	[Serializable]
	public class UserPropertySeeMoreFromUserButton : IUserProperty, IPropertyMonoBehaviourEvents
	{
		public void OnUserUpdate(UserProfile user)
		{
			this._user = user;
		}

		public void Start()
		{
		}

		public void OnDestroy()
		{
		}

		public void OnEnable()
		{
			this._button.onClick.AddListener(new UnityAction(this.OnClicked));
		}

		public void OnDisable()
		{
			this._button.onClick.RemoveListener(new UnityAction(this.OnClicked));
		}

		private void OnClicked()
		{
			if (this._user != null)
			{
				ModioUISearch.Default.SetSearchForUser(this._user);
				ModDisplayPanel panelOfType = ModioPanelManager.GetPanelOfType<ModDisplayPanel>();
				if (panelOfType != null && panelOfType.HasFocus)
				{
					panelOfType.ClosePanel();
				}
			}
		}

		[SerializeField]
		private Button _button;

		private UserProfile _user;
	}
}
