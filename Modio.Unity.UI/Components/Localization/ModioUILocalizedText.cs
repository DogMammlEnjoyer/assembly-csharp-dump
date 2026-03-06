using System;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components.Localization
{
	public class ModioUILocalizedText : MonoBehaviour
	{
		private void Reset()
		{
			this._tmpText = base.GetComponent<TMP_Text>();
		}

		private void OnEnable()
		{
			ModioUILocalizationManager.LanguageSet += this.UpdateText;
		}

		private void OnDisable()
		{
			ModioUILocalizationManager.LanguageSet -= this.UpdateText;
		}

		public void SetFormatArgs(params object[] args)
		{
			this._args = args;
			this.UpdateText();
		}

		private void UpdateText()
		{
			if (!string.IsNullOrEmpty(this._key))
			{
				string text = ModioUILocalizationManager.GetLocalizedText(this._key, true);
				TMP_Text[] splitFormatArgs = this._splitFormatArgs;
				if (splitFormatArgs != null && splitFormatArgs.Length != 0)
				{
					string[] array = text.Split(new string[]
					{
						"{0}"
					}, StringSplitOptions.None);
					this._splitFormatArgs[0].text = ((array.Length != 0) ? array[0] : "");
					if (this._splitFormatArgs.Length > 2)
					{
						this._splitFormatArgs[2].text = ((array.Length > 1) ? array[1] : "");
					}
					if (this._splitFormatArgs.Length > 1)
					{
						TMP_Text tmp_Text = this._splitFormatArgs[1];
						object[] args = this._args;
						string text2;
						if (args == null || args.Length == 0)
						{
							text2 = "";
						}
						else
						{
							object obj = this._args[0];
							text2 = ((obj != null) ? obj.ToString() : null);
						}
						tmp_Text.text = text2;
						return;
					}
				}
				else if (this._tmpText != null)
				{
					if (this._args != null)
					{
						text = string.Format(text, this._args);
					}
					this._tmpText.text = text;
				}
			}
		}

		public bool SetKeyIfItExists(string key)
		{
			if (!string.IsNullOrEmpty(ModioUILocalizationManager.GetLocalizedText(key, false)))
			{
				this.SetKey(key);
				return true;
			}
			return false;
		}

		public void SetKey(string key)
		{
			if (string.IsNullOrEmpty(this._initialKey))
			{
				this._initialKey = this._key;
			}
			this._key = key;
			this.UpdateText();
		}

		public void ResetKey()
		{
			if (!string.IsNullOrEmpty(this._initialKey))
			{
				this.SetKey(this._initialKey);
			}
		}

		public void SetKey(string key, params object[] args)
		{
			this._args = args;
			this.SetKey(key);
		}

		[SerializeField]
		private string _key;

		[SerializeField]
		private TMP_Text _tmpText;

		[SerializeField]
		private TMP_Text[] _splitFormatArgs;

		private object[] _args;

		private string _initialKey;
	}
}
