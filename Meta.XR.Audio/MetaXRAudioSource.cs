using System;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MetaXRAudioSource : MonoBehaviour
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void OnBeforeSceneLoadRuntimeMethod()
	{
		Debug.Log(string.Format("Setting spatial voice limit: {0}", MetaXRAudioSettings.Instance.voiceLimit));
		MetaXRAudioSource.MetaXRAudio_SetGlobalVoiceLimit(MetaXRAudioSettings.Instance.voiceLimit);
	}

	public bool EnableSpatialization
	{
		get
		{
			return this.enableSpatialization;
		}
		set
		{
			this.enableSpatialization = value;
		}
	}

	public float GainBoostDb
	{
		get
		{
			return this.gainBoostDb;
		}
		set
		{
			this.gainBoostDb = Mathf.Clamp(value, 0f, 20f);
		}
	}

	public bool EnableAcoustics
	{
		get
		{
			return this.enableAcoustics;
		}
		set
		{
			this.enableAcoustics = value;
		}
	}

	public float ReverbSendDb
	{
		get
		{
			return this.reverbSendDb;
		}
		set
		{
			this.reverbSendDb = Mathf.Clamp(value, -60f, 20f);
		}
	}

	private void Awake()
	{
		this.source_ = base.GetComponent<AudioSource>();
		this.UpdateParameters();
	}

	private void Update()
	{
		if (this.source_ == null)
		{
			this.source_ = base.GetComponent<AudioSource>();
			if (this.source_ == null)
			{
				return;
			}
		}
		this.UpdateParameters();
		this.wasPlaying_ = this.source_.isPlaying;
	}

	public void UpdateParameters()
	{
		this.source_.spatialize = this.enableSpatialization;
		this.source_.SetSpatializerFloat(0, this.gainBoostDb);
		this.source_.SetSpatializerFloat(5, this.enableAcoustics ? 0f : 1f);
		this.source_.SetSpatializerFloat(11, this.reverbSendDb);
	}

	[DllImport("MetaXRAudioUnity")]
	private static extern int MetaXRAudio_SetGlobalVoiceLimit(int VoiceLimit);

	private AudioSource source_;

	private bool wasPlaying_;

	[SerializeField]
	[Tooltip("Enables HRTF Spatialization.")]
	private bool enableSpatialization = true;

	[SerializeField]
	[Tooltip("Additional gain beyond 0dB")]
	[Range(0f, 20f)]
	private float gainBoostDb;

	[SerializeField]
	[Tooltip("Enables room acoustics simulation (early reflections and reverberation) for this audio source only")]
	private bool enableAcoustics = true;

	[SerializeField]
	[Tooltip("Additional gain applied to reverb send for this audio source only")]
	[Range(-60f, 20f)]
	private float reverbSendDb;

	public enum NativeParameterIndex
	{
		P_GAIN,
		P_USEINVSQR,
		P_NEAR,
		P_FAR,
		P_RADIUS,
		P_DISABLE_RFL,
		P_AMBISTAT,
		P_READONLY_GLOBAL_RFL_ENABLED,
		P_READONLY_NUM_VOICES,
		P_HRTF_INTENSITY,
		P_REFLECTIONS_SEND,
		P_REVERB_SEND,
		P_DIRECTIVITY_ENABLED,
		P_DIRECTIVITY_INTENSITY,
		P_AMBI_DIRECT_ENABLED,
		P_REVERB_REACH,
		P_DIRECT_ENABLED,
		P_OCCLUSION_INTENSITY,
		P_MEDIUM_ABSORPTION,
		P_NUM
	}
}
