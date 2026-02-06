using System;
using GorillaTag.Audio;
using UnityEngine;
using UnityEngine.Events;

public class SITouchscreenButton : MonoBehaviour, IClickable
{
	private bool IsReady
	{
		get
		{
			if (!this._screenRegion)
			{
				return Time.time - this._enableTime >= 0.2f;
			}
			return !this._screenRegion.HasPressedButton;
		}
	}

	public bool IsToggledOn
	{
		get
		{
			return this._isToggledOn;
		}
	}

	private void Awake()
	{
		ITouchScreenStation componentInParent = base.GetComponentInParent<ITouchScreenStation>();
		if (componentInParent != null)
		{
			this._screenRegion = componentInParent.ScreenRegion;
		}
		if (this.buttonMode == SITouchscreenButton.ButtonMode.Toggle)
		{
			this._isToggledOn = this._startToggledOn;
		}
	}

	private void OnEnable()
	{
		this._enableTime = Time.time;
	}

	private void OnTriggerEnter(Collider other)
	{
		GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent)
		{
			this.PressButton();
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	public void PressButton()
	{
		if (!this.IsReady || !this.isUsable)
		{
			return;
		}
		if (this._screenRegion)
		{
			this._screenRegion.RegisterButtonPress();
		}
		if (this.buttonMode == SITouchscreenButton.ButtonMode.Normal)
		{
			this.buttonPressed.Invoke(this.buttonType, this.data, NetworkSystem.Instance.LocalPlayer.ActorNumber);
		}
		else if (this.buttonMode == SITouchscreenButton.ButtonMode.Toggle)
		{
			bool arg = !this._isToggledOn;
			this.buttonToggled.Invoke(this.buttonType, this.data, NetworkSystem.Instance.LocalPlayer.ActorNumber, arg);
		}
		if (this._pressSound != null)
		{
			GTAudioOneShot.Play(this._pressSound, base.transform.position, this._pressSoundVolume, 1f);
		}
	}

	public void SetToggleState(bool state, bool invokeEvent = false)
	{
		if (this.buttonMode != SITouchscreenButton.ButtonMode.Toggle)
		{
			return;
		}
		bool flag = this._isToggledOn != state;
		this._isToggledOn = state;
		if (invokeEvent && flag)
		{
			this.buttonToggled.Invoke(this.buttonType, this.data, NetworkSystem.Instance.LocalPlayer.ActorNumber, this._isToggledOn);
		}
	}

	public void Click(bool leftHand = false)
	{
		this.PressButton();
	}

	public SITouchscreenButton.ButtonMode buttonMode;

	public SITouchscreenButton.SITouchscreenButtonType buttonType;

	public int data;

	[SerializeField]
	private AudioClip _pressSound;

	[SerializeField]
	private float _pressSoundVolume = 0.1f;

	[SerializeField]
	private bool _isToggledOn;

	[SerializeField]
	private bool _startToggledOn;

	public UnityEvent<SITouchscreenButton.SITouchscreenButtonType, int, int> buttonPressed;

	public UnityEvent<SITouchscreenButton.SITouchscreenButtonType, int, int, bool> buttonToggled;

	private SIScreenRegion _screenRegion;

	private const float DEBOUNCE_TIME = 0.2f;

	private float _enableTime;

	[NonSerialized]
	public bool isUsable = true;

	public enum ButtonMode
	{
		Normal,
		Toggle
	}

	public enum SITouchscreenButtonType
	{
		Back,
		Next,
		Exit,
		Help,
		Select,
		Dispense,
		Research,
		Collect,
		Debug,
		PageSelect,
		Purchase,
		Confirm,
		Cancel,
		OverrideFailure,
		None,
		Subscribe
	}
}
