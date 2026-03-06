using System;
using TMPro;
using UnityEngine;

namespace Oculus.Interaction
{
	public class UITheme : ScriptableObject
	{
		public int ThemeVersion
		{
			get
			{
				return this._themeVersion;
			}
		}

		private const int CurrentThemeVersion = 2;

		[SerializeField]
		[HideInInspector]
		private int _themeVersion = 2;

		public Color backplateColor;

		public Material backplateGradientMaterial;

		public Color buttonPlateColor;

		public Color sectionPlateColor;

		public Color tooltipColor;

		[Header("Shared")]
		public Color textPrimaryColor;

		public Color textSecondaryColor;

		public Color textPrimaryInvertedColor;

		public Color textSecondaryInvertedColor;

		[Header("Per Element Type Color")]
		public UITheme.ElementColors primaryButton;

		public UITheme.ElementColors secondaryButton;

		public UITheme.ElementColors borderlessButton;

		public UITheme.ElementColors destructiveButton;

		[Header("Fonts")]
		public TMP_FontAsset textFontBold;

		public TMP_FontAsset textFontMedium;

		public TMP_FontAsset textFontRegular;

		[Header("Animators")]
		public RuntimeAnimatorController acPrimaryButton;

		public RuntimeAnimatorController acSecondaryButton;

		public RuntimeAnimatorController acBorderlessButton;

		public RuntimeAnimatorController acDestructiveButton;

		public RuntimeAnimatorController acToggleButton;

		public RuntimeAnimatorController acToggleBorderlessButton;

		public RuntimeAnimatorController acToggleSwitch;

		public RuntimeAnimatorController acToggleCheckboxRadio;

		public RuntimeAnimatorController acTextInputField;

		[Space(10f)]
		public string colorPath = "Content/Background";

		[Serializable]
		public struct ElementColors
		{
			public Color normal;

			public Color highlighted;

			public Color pressed;

			public Color selected;

			public Color disabled;
		}
	}
}
