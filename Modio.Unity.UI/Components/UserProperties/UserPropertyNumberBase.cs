using System;
using Modio.Users;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components.UserProperties
{
	public abstract class UserPropertyNumberBase : IUserProperty
	{
		public void OnUserUpdate(UserProfile user)
		{
			this._text.text = StringFormat.Kilo(this._format, this.GetValue(user), this._customFormat);
		}

		protected abstract long GetValue(UserProfile user);

		private bool IsCustomFormat()
		{
			return this._format == StringFormatKilo.Custom;
		}

		[SerializeField]
		private TMP_Text _text;

		[SerializeField]
		[Tooltip("None: \"10500\".\r\nComma: \"10,500\".\r\nKilo: \"10.5k\".")]
		private StringFormatKilo _format = StringFormatKilo.Kilo;

		[SerializeField]
		[ShowIf("IsCustomFormat")]
		private string _customFormat;
	}
}
