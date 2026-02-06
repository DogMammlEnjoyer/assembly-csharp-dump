using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SITouchscreenButtonContainer : MonoBehaviour
{
	public bool isUsable { get; private set; }

	private void Start()
	{
		if (Application.isPlaying && this.button != null && this.button.buttonMode == SITouchscreenButton.ButtonMode.Toggle)
		{
			this.button.buttonToggled.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int, bool>(this.OnToggleStateChanged));
			this.UpdateToggleVisual(this.button.IsToggledOn);
		}
	}

	private void OnToggleStateChanged(SITouchscreenButton.SITouchscreenButtonType type, int data, int actorNr, bool isToggledOn)
	{
		this.UpdateToggleVisual(isToggledOn);
	}

	private void UpdateToggleVisual(bool isToggledOn)
	{
		if (this._cachedForegroundColor.r < 0f)
		{
			this._cachedForegroundColor = this.foreGround.color;
		}
		this.foreGround.color = (isToggledOn ? this.toggleOnColor : this.toggleOffColor);
		this.buttonText.text = (isToggledOn ? this.toggleOnText : this.toggleOffText);
	}

	public void SetUsable(bool newIsUsable)
	{
		if (this._cachedForegroundColor.r < 0f)
		{
			this._cachedForegroundColor = this.foreGround.color;
		}
		this.isUsable = newIsUsable;
		if (this.button.buttonMode == SITouchscreenButton.ButtonMode.Normal)
		{
			this.foreGround.color = (newIsUsable ? this._cachedForegroundColor : Color.gray);
		}
		this.button.isUsable = newIsUsable;
	}

	public SITouchscreenButton.SITouchscreenButtonType type;

	public string buttonTextString;

	public int data;

	public RectTransform backGround;

	public RectTransform backgroundShadow;

	public Image foreGround;

	public TextMeshProUGUI buttonText;

	public ITouchScreenStation station;

	[Header("Toggle Visual Settings")]
	public Color toggleOnColor = new Color(0f, 1f, 0.345098f);

	public Color toggleOffColor = new Color(0.5f, 0.5f, 0.5f);

	[Header("Toggle Text Settings")]
	[Tooltip("Text to display when toggle is ON")]
	public string toggleOnText = "ON";

	[Tooltip("Text to display when toggle is OFF")]
	public string toggleOffText = "OFF";

	public SITouchscreenButton button;

	[SerializeField]
	private bool autoConfigure = true;

	[NonSerialized]
	private Color _cachedForegroundColor = new Color(-1f, -1f, -1f);
}
