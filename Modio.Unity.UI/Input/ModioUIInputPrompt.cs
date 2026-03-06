using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Modio.Unity.UI.Input
{
	public class ModioUIInputPrompt : MonoBehaviour
	{
		private void Awake()
		{
			this._button = base.GetComponent<Button>();
			this._layoutElement = base.GetComponent<LayoutElement>();
			if (this._layoutElement != null)
			{
				this._layoutElementIgnoreLayout = this._layoutElement.ignoreLayout;
			}
		}

		private void OnEnable()
		{
			ModioUIInput.InputPromptDisplayInfo inputPromptDisplayInfo = ModioUIInput.GetInputPromptDisplayInfo(this._action);
			inputPromptDisplayInfo.OnUpdated += this.DisplayInfoUpdated;
			this.DisplayInfoUpdated(inputPromptDisplayInfo);
		}

		private void OnDisable()
		{
			ModioUIInput.GetInputPromptDisplayInfo(this._action).OnUpdated -= this.DisplayInfoUpdated;
		}

		public void PressedAction()
		{
			ModioUIInput.PressedAction(this._action);
		}

		private void DisplayInfoUpdated(ModioUIInput.InputPromptDisplayInfo info)
		{
			if (this._hideIfNoListener && !info.InputHasListeners)
			{
				this.<DisplayInfoUpdated>g__SetElementsVisible|16_0(false, false);
				return;
			}
			if ((this._hideIfController && ModioUIInput.IsUsingGamepad) || (this._hideIfNotController && !ModioUIInput.IsUsingGamepad))
			{
				this.<DisplayInfoUpdated>g__SetElementsVisible|16_0(false, false);
				return;
			}
			List<Sprite> icons = info.Icons;
			if (icons != null && icons.Count > 0)
			{
				this.<DisplayInfoUpdated>g__SetElementsVisible|16_0(false, true);
				this._image.sprite = info.Icons[0];
				return;
			}
			List<string> textPrompts = info.TextPrompts;
			if (textPrompts != null && textPrompts.Count > 0)
			{
				this.<DisplayInfoUpdated>g__SetElementsVisible|16_0(true, false);
				if (this._inputPromptText != null)
				{
					this._inputPromptText.text = info.TextPrompts[0];
					return;
				}
			}
			else
			{
				if (this._hideIfNoBindings)
				{
					this.<DisplayInfoUpdated>g__SetElementsVisible|16_0(false, false);
					return;
				}
				if (!ModioUIInput.AnyBindingsExist)
				{
					this.<DisplayInfoUpdated>g__SetElementsVisible|16_0(false, false);
					return;
				}
				if (this._inputPromptText != null)
				{
					this._inputPromptText.text = "UNBOUND";
				}
				this.<DisplayInfoUpdated>g__SetElementsVisible|16_0(true, false);
			}
		}

		[CompilerGenerated]
		private void <DisplayInfoUpdated>g__SetElementsVisible|16_0(bool textVisible, bool imageVisible)
		{
			if (this._inputPromptText != null)
			{
				this._inputPromptText.gameObject.SetActive(textVisible);
			}
			if (this._textBackground != null)
			{
				this._textBackground.gameObject.SetActive(textVisible);
			}
			if (this._image != null)
			{
				this._image.gameObject.SetActive(imageVisible);
			}
			bool flag = textVisible || imageVisible;
			if (this._button != null)
			{
				this._button.interactable = flag;
			}
			if (this._layoutElement != null)
			{
				this._layoutElement.ignoreLayout = (this._layoutElementIgnoreLayout || !flag);
			}
			GameObject[] additionalToHideIfNoBindings = this._additionalToHideIfNoBindings;
			for (int i = 0; i < additionalToHideIfNoBindings.Length; i++)
			{
				additionalToHideIfNoBindings[i].SetActive(flag);
			}
		}

		[SerializeField]
		private ModioUIInput.ModioAction _action;

		[FormerlySerializedAs("_text")]
		[SerializeField]
		private TMP_Text _inputPromptText;

		[SerializeField]
		private Image _textBackground;

		[SerializeField]
		private Image _image;

		[SerializeField]
		private bool _hideIfNoBindings;

		[SerializeField]
		private bool _hideIfNoListener;

		[SerializeField]
		private bool _hideIfController;

		[SerializeField]
		private bool _hideIfNotController;

		[SerializeField]
		private GameObject[] _additionalToHideIfNoBindings;

		private Button _button;

		private LayoutElement _layoutElement;

		private bool _layoutElementIgnoreLayout;
	}
}
