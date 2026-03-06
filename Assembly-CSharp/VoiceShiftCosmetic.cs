using System;
using UnityEngine;

public class VoiceShiftCosmetic : MonoBehaviour
{
	public bool ModifyPitch
	{
		get
		{
			return this.modifyPitch;
		}
	}

	public bool ModifyVolume
	{
		get
		{
			return this.modifyVolume;
		}
	}

	public bool IsShifted
	{
		get
		{
			return this.isShifted;
		}
	}

	public float Pitch
	{
		get
		{
			return this.pitch;
		}
		set
		{
			if (!this.modifyPitch)
			{
				return;
			}
			float num = Mathf.Clamp(value, 0.6666667f, 1.5f);
			this.pitch = num;
			VRRig vrrig = this.myRig;
			if (vrrig == null)
			{
				return;
			}
			vrrig.SetVoiceShiftCosmeticsDirty();
		}
	}

	public float Volume
	{
		get
		{
			return this.volume;
		}
		set
		{
			if (!this.modifyVolume)
			{
				return;
			}
			float num = Mathf.Clamp(value, 0f, 1f);
			this.volume = num;
			VRRig vrrig = this.myRig;
			if (vrrig == null)
			{
				return;
			}
			vrrig.SetVoiceShiftCosmeticsDirty();
		}
	}

	private void OnEnable()
	{
		if (this.myRig == null)
		{
			this.myRig = base.GetComponentInParent<VRRig>();
		}
		if (this.myRig == null)
		{
			return;
		}
		this.myRig.VoiceShiftCosmetics.Add(this);
		this.myRig.SetVoiceShiftCosmeticsDirty();
	}

	private void OnDisable()
	{
		if (this.myRig == null)
		{
			return;
		}
		this.myRig.VoiceShiftCosmetics.Remove(this);
		this.myRig.SetVoiceShiftCosmeticsDirty();
	}

	public void StartVoiceShift()
	{
		if (this.isShifted)
		{
			return;
		}
		this.isShifted = true;
		if (this.modifyPitch)
		{
			this.Pitch = this.shiftedPitch;
		}
		if (this.modifyVolume)
		{
			this.Volume = this.shiftedVolume;
		}
	}

	public void StopVoiceShift()
	{
		if (!this.isShifted)
		{
			return;
		}
		this.isShifted = false;
		VRRig vrrig = this.myRig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.SetVoiceShiftCosmeticsDirty();
	}

	public void ToggleVoiceShift()
	{
		if (this.isShifted)
		{
			this.StopVoiceShift();
			return;
		}
		this.StartVoiceShift();
	}

	private const float PITCH_MIN = 0.6666667f;

	private const float PITCH_MAX = 1.5f;

	private const float VOLUME_MIN = 0f;

	private const float VOLUME_MAX = 1f;

	[SerializeField]
	private bool modifyPitch = true;

	[SerializeField]
	private bool modifyVolume = true;

	[Range(0.6666667f, 1.5f)]
	[SerializeField]
	private float shiftedPitch = 1.5f;

	[Range(0f, 1f)]
	[SerializeField]
	private float shiftedVolume = 1f;

	private float pitch = 1f;

	private float volume = 1f;

	private bool isShifted;

	private VRRig myRig;
}
