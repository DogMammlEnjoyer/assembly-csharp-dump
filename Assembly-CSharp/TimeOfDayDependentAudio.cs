using System;
using UnityEngine;

public class TimeOfDayDependentAudio : MonoBehaviour, IGorillaSliceableSimple, IBuildValidation
{
	private void Awake()
	{
		this.stepTime = 1f;
		if (this.myParticleSystem != null)
		{
			this.myEmissionModule = this.myParticleSystem.emission;
			this.startingEmissionRate = this.myEmissionModule.rateOverTime.constant;
		}
		if (this.isModified)
		{
			this.positionMultiplier = this.positionMultiplierSet;
		}
		else
		{
			this.positionMultiplier = 1f;
		}
		if (this.volumes == null)
		{
			this.volumes = new float[10];
		}
	}

	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.FixedUpdate);
	}

	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.FixedUpdate);
	}

	public void SliceUpdate()
	{
		this.isModified = false;
		this.UpdateTimeOfDay();
	}

	private void UpdateTimeOfDay()
	{
		if (BetterDayNightManager.instance == null)
		{
			return;
		}
		BetterDayNightManager.WeatherType weatherType = BetterDayNightManager.instance.CurrentWeather();
		BetterDayNightManager.WeatherType weatherType2 = BetterDayNightManager.instance.NextWeather();
		bool flag = this.myWeather == BetterDayNightManager.WeatherType.All || this.myWeather == weatherType || this.myWeather == weatherType2;
		bool flag2 = this.myWeather != BetterDayNightManager.WeatherType.All && weatherType != weatherType2;
		int currentTimeIndex = BetterDayNightManager.instance.currentTimeIndex;
		int num = (currentTimeIndex + 1) % BetterDayNightManager.instance.timeOfDayRange.Length;
		int num2 = (currentTimeIndex - 1) % BetterDayNightManager.instance.timeOfDayRange.Length;
		if (num2 < 0)
		{
			num2 = BetterDayNightManager.instance.timeOfDayRange.Length - 1;
		}
		float currentLerp = BetterDayNightManager.instance.currentLerp;
		if (!flag)
		{
			if (this.dependentStuff.activeSelf)
			{
				this.dependentStuff.SetActive(false);
			}
			return;
		}
		if (!this.dependentStuff.activeSelf && (!this.includesAudio || this.dependentStuff != this.timeOfDayDependent))
		{
			this.dependentStuff.SetActive(true);
		}
		if (this.includesAudio && this.timeOfDayDependent != null)
		{
			bool flag3 = this.volumes[currentTimeIndex] != 0f;
			if (this.timeOfDayDependent.activeSelf != flag3)
			{
				this.timeOfDayDependent.SetActive(flag3);
			}
		}
		if (!flag2)
		{
			this.newRate = this.startingEmissionRate;
			this.currentVolume = Mathf.Lerp(this.volumes[num2], this.volumes[currentTimeIndex], Mathf.Clamp(currentLerp * 20f, 0f, 1f));
		}
		else if (this.myWeather == weatherType2)
		{
			float t = Mathf.Clamp(currentLerp * 2f - 1f, 0f, 1f);
			this.newRate = Mathf.Lerp(0f, this.startingEmissionRate, t);
			this.currentVolume = Mathf.Lerp(0f, this.volumes[num], currentLerp);
		}
		else
		{
			float t2 = Mathf.Clamp(currentLerp * 2f, 0f, 1f);
			this.newRate = Mathf.Lerp(this.startingEmissionRate, 0f, t2);
			this.currentVolume = Mathf.Lerp(this.volumes[currentTimeIndex], 0f, currentLerp);
		}
		if (this.myParticleSystem != null)
		{
			this.myEmissionModule = this.myParticleSystem.emission;
			this.myEmissionModule.rateOverTime = this.newRate;
			bool flag4 = this.newRate != 0f;
			if (this.myParticleSystem.gameObject.activeSelf != flag4)
			{
				this.myParticleSystem.gameObject.SetActive(flag4);
			}
		}
		if (this.includesAudio)
		{
			for (int i = 0; i < this.audioSources.Length; i++)
			{
				MusicSource component = this.audioSources[i].gameObject.GetComponent<MusicSource>();
				if (!(component != null) || !component.VolumeOverridden)
				{
					this.audioSources[i].volume = this.currentVolume * this.positionMultiplier;
					this.audioSources[i].enabled = (this.currentVolume != 0f);
				}
			}
		}
	}

	public bool BuildValidationCheck()
	{
		for (int i = 0; i < this.audioSources.Length; i++)
		{
			if (this.audioSources[i] == null)
			{
				Debug.LogError("audio source array contains null references", this);
				return false;
			}
		}
		if (this.volumes.Length != 10)
		{
			Debug.LogError("volumes array is the wrong length! you'll pay for this!", this);
			return false;
		}
		return true;
	}

	public AudioSource[] audioSources;

	public float[] volumes;

	public float currentVolume;

	public float stepTime;

	public BetterDayNightManager.WeatherType myWeather;

	public GameObject dependentStuff;

	public GameObject timeOfDayDependent;

	public bool includesAudio;

	public ParticleSystem myParticleSystem;

	private float startingEmissionRate;

	private ParticleSystem.MinMaxCurve newCurve;

	private ParticleSystem.EmissionModule myEmissionModule;

	private float newRate;

	public float positionMultiplierSet;

	public float positionMultiplier = 1f;

	public bool isModified;
}
