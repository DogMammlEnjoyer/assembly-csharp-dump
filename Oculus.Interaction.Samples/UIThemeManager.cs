using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction
{
	public class UIThemeManager : MonoBehaviour
	{
		public UITheme[] Themes
		{
			get
			{
				return this._themes;
			}
		}

		public int CurrentThemeIndex
		{
			get
			{
				return this._currentThemeIndex;
			}
		}

		private void Start()
		{
			this.ApplyTheme(this._currentThemeIndex);
		}

		public void ApplyCurrentTheme()
		{
			this.ApplyTheme(this._currentThemeIndex);
		}

		public void ApplyTheme(int index)
		{
			if (index < 0 || index >= this._themes.Length)
			{
				Debug.LogError("Theme index out of range.");
				return;
			}
			this._currentThemeIndex = index;
			UITheme uitheme = this._themes[index];
			foreach (Animator animator in base.GetComponentsInChildren<Animator>())
			{
				if (animator.CompareTag("QDSUIPrimaryButton"))
				{
					animator.runtimeAnimatorController = uitheme.acPrimaryButton;
				}
				else if (animator.CompareTag("QDSUISecondaryButton"))
				{
					animator.runtimeAnimatorController = uitheme.acSecondaryButton;
				}
				else if (animator.CompareTag("QDSUIBorderlessButton"))
				{
					animator.runtimeAnimatorController = uitheme.acBorderlessButton;
				}
				else if (animator.CompareTag("QDSUIDestructiveButton"))
				{
					animator.runtimeAnimatorController = uitheme.acDestructiveButton;
				}
				else if (animator.CompareTag("QDSUIToggleButton"))
				{
					animator.runtimeAnimatorController = uitheme.acToggleButton;
				}
				else if (animator.CompareTag("QDSUIToggleBorderlessButton"))
				{
					animator.runtimeAnimatorController = uitheme.acToggleBorderlessButton;
				}
				else if (animator.CompareTag("QDSUIToggleSwitch"))
				{
					animator.runtimeAnimatorController = uitheme.acToggleSwitch;
				}
				else if (animator.CompareTag("QDSUIToggleCheckboxRadio"))
				{
					animator.runtimeAnimatorController = uitheme.acToggleCheckboxRadio;
				}
				else if (animator.CompareTag("QDSUITextInputField"))
				{
					animator.runtimeAnimatorController = uitheme.acTextInputField;
				}
				animator.Update(0f);
			}
			foreach (Image image in base.GetComponentsInChildren<Image>())
			{
				if (image.CompareTag("QDSUIIcon"))
				{
					image.color = uitheme.textPrimaryColor;
				}
				else if (image.CompareTag("QDSUIAccentColor"))
				{
					image.color = uitheme.primaryButton.normal;
				}
				else if (!image.CompareTag("QDSUISharedThemeColor") && !image.CompareTag("QDSUIDestructiveButton") && !image.CompareTag("QDSUIBorderlessButton") && !image.CompareTag("QDSUIToggleBorderlessButton") && !image.CompareTag("QDSUISecondaryButton") && !image.CompareTag("QDSUIToggleButton"))
				{
					if (image.CompareTag("QDSUISection"))
					{
						image.color = uitheme.sectionPlateColor;
					}
					else if (image.CompareTag("QDSUITooltip"))
					{
						image.color = uitheme.tooltipColor;
					}
					else if (!image.CompareTag("QDSUITextInputField"))
					{
						image.color = uitheme.backplateColor;
					}
				}
				if (uitheme.ThemeVersion == 2)
				{
					if (image.CompareTag("QDSUIBackplateGradient"))
					{
						image.color = uitheme.backplateColor;
						image.material = uitheme.backplateGradientMaterial;
					}
					if (image.CompareTag("QDSUITextInvertedColor"))
					{
						image.color = uitheme.textPrimaryInvertedColor;
					}
				}
			}
			SpriteRenderer[] componentsInChildren3 = base.GetComponentsInChildren<SpriteRenderer>();
			for (int i = 0; i < componentsInChildren3.Length; i++)
			{
				componentsInChildren3[i].color = uitheme.tooltipColor;
			}
			foreach (TextMeshProUGUI textMeshProUGUI in base.GetComponentsInChildren<TextMeshProUGUI>())
			{
				textMeshProUGUI.font = uitheme.textFontMedium;
				if (!textMeshProUGUI.CompareTag("QDSUISharedThemeColor") && !textMeshProUGUI.CompareTag("QDSUIDestructiveButton"))
				{
					if (textMeshProUGUI.CompareTag("QDSUITextSecondaryColor"))
					{
						textMeshProUGUI.color = uitheme.textSecondaryColor;
					}
					else
					{
						textMeshProUGUI.color = uitheme.textPrimaryColor;
					}
				}
				if (uitheme.ThemeVersion == 2)
				{
					if (textMeshProUGUI.CompareTag("QDSUITextInvertedColor"))
					{
						textMeshProUGUI.color = uitheme.textPrimaryInvertedColor;
					}
					if (textMeshProUGUI.CompareTag("QDSUITextSecondaryInvertedColor"))
					{
						textMeshProUGUI.color = uitheme.textSecondaryInvertedColor;
					}
				}
			}
		}

		[SerializeField]
		private UITheme[] _themes;

		[SerializeField]
		private int _currentThemeIndex;
	}
}
