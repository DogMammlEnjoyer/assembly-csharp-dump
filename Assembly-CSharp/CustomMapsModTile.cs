using System;
using System.Runtime.CompilerServices;
using Modio.Mods;
using TMPro;
using UnityEngine;

public class CustomMapsModTile : CustomMapsScreenTouchPoint
{
	public Mod CurrentMod
	{
		get
		{
			return this.currentMod;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		this.defaultLogo = this.touchPointRenderer.sprite;
		this.highlight.SetActive(false);
	}

	public void ShowTileText(bool show, bool useMapName)
	{
		if (!show)
		{
			this.ratingsText.gameObject.SetActive(false);
			this.mapNameText.gameObject.SetActive(false);
			this.thumsbUp.SetActive(false);
			return;
		}
		if (useMapName)
		{
			this.mapNameText.gameObject.SetActive(true);
			this.ratingsText.gameObject.SetActive(false);
			this.thumsbUp.SetActive(false);
			return;
		}
		this.ratingsText.gameObject.SetActive(true);
		this.thumsbUp.SetActive(true);
		this.mapNameText.gameObject.SetActive(false);
	}

	public void ActivateTile(bool useMapName)
	{
		this.isActive = true;
		base.gameObject.SetActive(true);
		this.ShowTileText(true, useMapName);
		CustomMapsScreenTouchPoint.pressTime = Time.time;
	}

	public void DeactivateTile()
	{
		this.isActive = false;
		base.gameObject.SetActive(false);
		this.highlight.SetActive(false);
		this.ShowTileText(false, false);
		this.ResetLogo();
	}

	public override void PressButtonColourUpdate()
	{
	}

	protected override void OnButtonPressedEvent()
	{
	}

	public void SetMod(Mod mod, bool useMapName)
	{
		CustomMapsModTile.<SetMod>d__19 <SetMod>d__;
		<SetMod>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SetMod>d__.<>4__this = this;
		<SetMod>d__.mod = mod;
		<SetMod>d__.useMapName = useMapName;
		<SetMod>d__.<>1__state = -1;
		<SetMod>d__.<>t__builder.Start<CustomMapsModTile.<SetMod>d__19>(ref <SetMod>d__);
	}

	public void ResetLogo()
	{
		this.touchPointRenderer.sprite = this.defaultLogo;
	}

	public void ShowDetails()
	{
		CustomMapsTerminal.ShowDetailsScreen(this.currentMod);
	}

	public void HighlightTile()
	{
		this.highlight.SetActive(true);
	}

	public bool IsCurrentModHidden()
	{
		return this.currentMod.Creator == null || (!ModIOManager.IsLoggedIn() && this.currentMod.IsHidden());
	}

	[SerializeField]
	private TMP_Text ratingsText;

	[SerializeField]
	private TMP_Text mapNameText;

	[SerializeField]
	private GameObject thumsbUp;

	[SerializeField]
	private GameObject highlight;

	private const float LOGO_WIDTH = 320f;

	private const float LOGO_HEIGHT = 180f;

	private Mod currentMod;

	private Sprite defaultLogo;

	private bool isDownloadingThumbnail;

	private bool newDownloadRequest;

	private bool isActive;
}
