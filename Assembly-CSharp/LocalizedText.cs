using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Events;

[DisallowMultipleComponent]
public class LocalizedText : LocalizeStringEvent
{
	public bool HasFontOverrides()
	{
		return this._localisationFontsOverrides.Count > 0;
	}

	private TextComponentLegacySupportStore TextComponent
	{
		get
		{
			if (!this._textComponent.IsValid)
			{
				this._textComponent = new TextComponentLegacySupportStore(base.transform);
			}
			return this._textComponent;
		}
	}

	private void Awake()
	{
		this._textComponent = new TextComponentLegacySupportStore(base.transform);
		base.OnUpdateString = new UnityEventString();
		base.OnUpdateString.AddListener(delegate(string val)
		{
			this.OnLocaleChanged(val);
		});
		if (!this.TextComponent.IsValid)
		{
			base.gameObject.AddComponent<TMP_Text>();
			this._textComponent = new TextComponentLegacySupportStore(base.transform);
		}
	}

	protected override void UpdateString(string value)
	{
		LocalizedText.<UpdateString>d__11 <UpdateString>d__;
		<UpdateString>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<UpdateString>d__.<>4__this = this;
		<UpdateString>d__.value = value;
		<UpdateString>d__.<>1__state = -1;
		<UpdateString>d__.<>t__builder.Start<LocalizedText.<UpdateString>d__11>(ref <UpdateString>d__);
	}

	private void OnLocaleChanged(string newText)
	{
		LocalizedText.<OnLocaleChanged>d__12 <OnLocaleChanged>d__;
		<OnLocaleChanged>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnLocaleChanged>d__.<>4__this = this;
		<OnLocaleChanged>d__.newText = newText;
		<OnLocaleChanged>d__.<>1__state = -1;
		<OnLocaleChanged>d__.<>t__builder.Start<LocalizedText.<OnLocaleChanged>d__12>(ref <OnLocaleChanged>d__);
	}

	private bool GetLocalizedFonts(out LocalisationFontPair fontData)
	{
		fontData = default(LocalisationFontPair);
		if (!this.HasFontOverrides())
		{
			return LocalisationManager.GetFontAssetForCurrentLocale(out fontData);
		}
		for (int i = 0; i < this._localisationFontsOverrides.Count; i++)
		{
			if (this._localisationFontsOverrides[i].ContainsLocale(LocalisationManager.CurrentLanguage))
			{
				fontData = new LocalisationFontPair
				{
					fontAsset = this._localisationFontsOverrides[i].fontAsset,
					legacyFontAsset = this._localisationFontsOverrides[i].legacyFontAsset,
					charSpacing = this._localisationFontsOverrides[i].charSpacing
				};
				return true;
			}
		}
		return LocalisationManager.GetFontAssetForCurrentLocale(out fontData);
	}

	[SerializeField]
	private bool _isLocalized;

	[SerializeField]
	private bool _isNewKey;

	[SerializeField]
	private string _newKeyName;

	[SerializeField]
	private ELocale _previewLocale;

	[SerializeField]
	private List<LocalisationFontPair> _localisationFontsOverrides = new List<LocalisationFontPair>();

	private static List<ELocale> _cachedELocalesList = new List<ELocale>();

	private TextComponentLegacySupportStore _textComponent;
}
