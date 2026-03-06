using System;
using UnityEngine;

public class HatcheryEventFlashlight : MonoBehaviourTick, IGorillaSliceableSimple
{
	private void Awake()
	{
		this.parentRig = base.GetComponentInParent<VRRig>();
		this.playerLight = this.parentRig.isOfflineVRRig;
		this.currentEnergy = 10f;
		this.lightComponents = new Light[this.lights.Length];
		this.gameLightComponents = new GameLight[this.lights.Length];
		for (int i = 0; i < this.lights.Length; i++)
		{
			this.lightComponents[i] = this.lights[i].GetComponent<Light>();
			this.gameLightComponents[i] = this.lights[i].GetComponent<GameLight>();
		}
		this.startingBrightness = this.lightComponents[0].intensity;
		this.lightsParent.gameObject.SetActive(false);
	}

	private new void OnEnable()
	{
		base.OnEnable();
		if (!this.playerLight)
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this);
		}
	}

	private new void OnDisable()
	{
		base.OnDisable();
		if (!this.playerLight)
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this);
		}
	}

	private float MaxEnergy()
	{
		if (NetworkSystem.Instance.CurrentRoom == null)
		{
			return 10f;
		}
		return 10f * (1f / Mathf.Log((float)NetworkSystem.Instance.RoomPlayerCount + 1.72f));
	}

	public override void Tick()
	{
		if (this.playerLight)
		{
			this.SliceUpdate();
		}
	}

	public void SliceUpdate()
	{
		if (GameLightingManager.instance.IsDynamicLightingEnabled != this.flashlight.gameObject.activeSelf)
		{
			this.flashlight.gameObject.SetActive(GameLightingManager.instance.IsDynamicLightingEnabled);
		}
		if (!GameLightingManager.instance.IsDynamicLightingEnabled)
		{
			return;
		}
		float time = Time.time;
		float num = this.MaxEnergy();
		if (this.wasLightEnabled)
		{
			this.currentEnergy -= (time - this.lastUpdated) * 1f;
		}
		else
		{
			this.currentEnergy += (time - this.lastUpdated) * 0.66f;
		}
		this.currentEnergy = Mathf.Clamp(this.currentEnergy, 0f, this.MaxEnergy());
		bool flag = this.parentRig.rightIndex.calcT >= 0.33f;
		bool flag2 = flag && (!this.wasLightSwitchedOn || this.wasLightEnabled) && this.currentEnergy > 0f;
		if (flag2 != this.wasLightEnabled)
		{
			this.lightsParent.gameObject.SetActive(flag2);
			this.clickSource.Play();
		}
		if (flag2)
		{
			this.UpdateLightPositioning();
			this.UpdateLightBrightness(num);
		}
		this.lastUpdated = Time.time;
		this.wasLightSwitchedOn = flag;
		this.wasLightEnabled = flag2;
	}

	private void UpdateLightPositioning()
	{
		int num = Physics.RaycastNonAlloc(this.lightStart.position, this.lightStart.forward, this.hits, 6f, -1, QueryTriggerInteraction.Ignore);
		float num2 = 6f;
		for (int i = 0; i < num; i++)
		{
			if (this.hits[i].distance <= num2)
			{
				num2 = this.hits[i].distance;
			}
		}
		float num3 = (num2 >= 2f) ? (num2 - 1f) : (num2 / 2f);
		for (int j = 0; j < this.lights.Length; j++)
		{
			this.lights[j].position = this.lightStart.position + this.lightStart.forward * (num3 * (float)(j + 1) / (float)this.lights.Length);
		}
	}

	private void UpdateLightBrightness(float _maxEnergy)
	{
		float intensity = this.startingBrightness / 5f * (1f + 4f * this.currentEnergy / _maxEnergy);
		for (int i = 0; i < this.lightComponents.Length; i++)
		{
			this.lightComponents[i].intensity = intensity;
			this.gameLightComponents[i].UpdateCachedLightColorAndIntensity();
		}
	}

	private const float lightMaxDistance = 6f;

	private const float surfaceOffset = 1f;

	private const float enableThresholdCurl = 0.33f;

	private const float maxEnergy = 10f;

	private const float energyUsageRate = 1f;

	private const float energyChargeRate = 0.66f;

	private RaycastHit[] hits = new RaycastHit[20];

	private Light[] lightComponents;

	private GameLight[] gameLightComponents;

	private VRRig parentRig;

	private float currentEnergy;

	private float startingBrightness;

	private float lastUpdated;

	private bool playerLight;

	private bool wasLightEnabled;

	private bool wasLightSwitchedOn;

	public Transform lightStart;

	public Transform lightsParent;

	public Transform flashlight;

	public Transform[] lights;

	public AudioSource clickSource;
}
