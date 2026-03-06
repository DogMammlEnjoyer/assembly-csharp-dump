using System;
using Modio.Users;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components.UserProperties
{
	[Serializable]
	public class UserPropertyWalletAmount : IUserProperty, IPropertyMonoBehaviourEvents
	{
		public void OnUserUpdate(UserProfile user)
		{
			User user2 = User.Current;
			Wallet wallet = (user2 != null) ? user2.Wallet : null;
			this._text.text = ((wallet != null) ? wallet.Balance.ToString() : "");
			this.hasSetText = true;
		}

		public void Start()
		{
		}

		public void OnDestroy()
		{
		}

		public void OnEnable()
		{
			if (!this.hasSetText)
			{
				this._text.text = "";
			}
		}

		public void OnDisable()
		{
		}

		[SerializeField]
		private TMP_Text _text;

		private bool hasSetText;
	}
}
