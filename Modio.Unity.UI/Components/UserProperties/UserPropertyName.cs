using System;
using Modio.Unity.UI.Components.Localization;
using Modio.Users;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components.UserProperties
{
	[Serializable]
	public class UserPropertyName : IUserProperty
	{
		public void OnUserUpdate(UserProfile user)
		{
			if (user != null && user.Username != null)
			{
				if (this._localisedText != null)
				{
					this._localisedText.SetFormatArgs(new object[]
					{
						user.Username
					});
					return;
				}
				string text = user.Username;
				if (!string.IsNullOrEmpty(this._userLoggedInFormat))
				{
					text = string.Format(this._userLoggedInFormat, text);
				}
				this._text.text = text;
				return;
			}
			else
			{
				if (this._localisedText != null)
				{
					this._localisedText.SetFormatArgs(new object[]
					{
						""
					});
					return;
				}
				if (this._text != null)
				{
					this._text.text = this._noUserLoggedIn;
				}
				return;
			}
		}

		[SerializeField]
		private TMP_Text _text;

		[SerializeField]
		private ModioUILocalizedText _localisedText;

		[SerializeField]
		private string _userLoggedInFormat = "{0}";

		[SerializeField]
		private string _noUserLoggedIn;
	}
}
