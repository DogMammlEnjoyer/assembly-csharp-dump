using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SITouchscreenButtonContainer : MonoBehaviour
{
	public bool isUsable { get; private set; }

	public void SetUsable(bool newIsUsable)
	{
		if (this._cachedForegroundColor.r < 0f)
		{
			this._cachedForegroundColor = this.foreGround.color;
		}
		this.isUsable = newIsUsable;
		this.foreGround.color = (newIsUsable ? this._cachedForegroundColor : Color.gray);
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

	public SITouchscreenButton button;

	[SerializeField]
	private bool autoConfigure = true;

	[NonSerialized]
	private Color _cachedForegroundColor = new Color(-1f, -1f, -1f);
}
