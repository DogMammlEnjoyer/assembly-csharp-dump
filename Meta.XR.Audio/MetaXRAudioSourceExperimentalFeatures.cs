using System;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(MetaXRAudioSource))]
public class MetaXRAudioSourceExperimentalFeatures : MonoBehaviour
{
	public float HrtfIntensity
	{
		get
		{
			return this.hrtfIntensity;
		}
		set
		{
			this.hrtfIntensity = Mathf.Clamp(value, 0f, 1f);
		}
	}

	public float VolumetricRadius
	{
		get
		{
			return this.volumetricRadius;
		}
		set
		{
			this.volumetricRadius = Mathf.Max(value, 0f);
		}
	}

	public float EarlyReflectionsSendDb
	{
		get
		{
			return this.earlyReflectionsSendDb;
		}
		set
		{
			this.earlyReflectionsSendDb = Mathf.Clamp(value, -60f, 20f);
		}
	}

	public float ReverbReach
	{
		get
		{
			return this.reverbReach;
		}
		set
		{
			this.reverbReach = Mathf.Clamp(value, 0f, 1f);
		}
	}

	public float OcclusionIntensity
	{
		get
		{
			return this.occlusionIntensity;
		}
		set
		{
			this.occlusionIntensity = Mathf.Clamp(value, 0f, 1f);
		}
	}

	public float DirectivityIntensity
	{
		get
		{
			return this.directivityIntensity;
		}
		set
		{
			this.directivityIntensity = Mathf.Clamp(value, 0f, 1f);
		}
	}

	public MetaXRAudioSourceExperimentalFeatures.DirectivityPatternType DirectivityPattern
	{
		get
		{
			return this.directivityPattern;
		}
		set
		{
			this.directivityPattern = value;
		}
	}

	public bool DirectSoundEnabled
	{
		get
		{
			return this.directSoundEnabled;
		}
		set
		{
			this.directSoundEnabled = value;
		}
	}

	public bool MediumAbsorption
	{
		get
		{
			return this.mediumAbsorption;
		}
		set
		{
			this.mediumAbsorption = value;
		}
	}

	private void OnValidate()
	{
		this.volumetricRadius = Mathf.Max(this.volumetricRadius, 0f);
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
	}

	public void UpdateParameters()
	{
		this.source_.SetSpatializerFloat(9, this.hrtfIntensity);
		this.source_.SetSpatializerFloat(13, this.directivityIntensity);
		this.source_.SetSpatializerFloat(4, this.volumetricRadius);
		this.source_.SetSpatializerFloat(10, this.earlyReflectionsSendDb);
		this.source_.SetSpatializerFloat(12, (this.directivityPattern == MetaXRAudioSourceExperimentalFeatures.DirectivityPatternType.None) ? 0f : 1f);
		this.source_.SetSpatializerFloat(15, this.reverbReach);
		this.source_.SetSpatializerFloat(16, this.directSoundEnabled ? 1f : 0f);
		this.source_.SetSpatializerFloat(17, this.occlusionIntensity);
		this.source_.SetSpatializerFloat(18, this.mediumAbsorption ? 1f : 0f);
	}

	[DllImport("MetaXRAudioUnity")]
	private static extern void MetaXRAudio_GetGlobalRoomReflectionValues(ref bool reflOn, ref bool reverbOn, ref float width, ref float height, ref float length);

	private AudioSource source_;

	[SerializeField]
	[Tooltip("How much of the HRTF EQ is applied to the sound. Interaural time delay (ITD) and interaural level differences (ILD) are kept the same.")]
	[Range(0f, 1f)]
	private float hrtfIntensity = 1f;

	[SerializeField]
	[Tooltip("Used to increase the spatial audio emitter radius. Useful for sounds that come from a large area rather than a precise point. If increased too large, users may end up inside the radius if the sound source is too close.")]
	private float volumetricRadius;

	[SerializeField]
	[Tooltip("Additional gain applied to early reflections for this audio source only")]
	[Range(-60f, 20f)]
	private float earlyReflectionsSendDb;

	[SerializeField]
	[Tooltip("Adjust how much the direct-to-reverberant ratio increases with distance")]
	[Range(0f, 1f)]
	private float reverbReach = 0.5f;

	[SerializeField]
	[Tooltip("Adjust how much the direct-to-reverberant ratio increases with distance")]
	[Range(0f, 1f)]
	private float occlusionIntensity = 1f;

	[SerializeField]
	[Tooltip("Intensity controller for Directvity , Value of 1 will apply full directivity")]
	[Range(0f, 1f)]
	private float directivityIntensity = 1f;

	[SerializeField]
	[Tooltip("Option for human voice directivity pattern that makes this sound more muffled when the source is facing away from listener")]
	private MetaXRAudioSourceExperimentalFeatures.DirectivityPatternType directivityPattern;

	[SerializeField]
	[Tooltip("This switch can disable direct sound propagation, so only late reverberations is heard from this source")]
	private bool directSoundEnabled = true;

	[SerializeField]
	[Tooltip("This switch can disable direct sound propagation, so only late reverberations is heard from this source")]
	private bool mediumAbsorption = true;

	public enum DirectivityPatternType
	{
		None,
		HumanVoice
	}
}
