using System;
using UnityEngine;

public class GameLightPulse : GameLight, IGorillaSliceableSimple
{
	public new void Awake()
	{
		base.Awake();
		this.startingIntensity = this.light.intensity;
		this.offsetTime = Random.value / this.frequency;
	}

	protected new void OnEnable()
	{
		base.OnEnable();
		GorillaSlicerSimpleManager.RegisterSliceable(this);
	}

	protected new void OnDisable()
	{
		base.OnDisable();
		GorillaSlicerSimpleManager.UnregisterSliceable(this);
	}

	public void SliceUpdate()
	{
		this.light.intensity = this.startingIntensity / 2f * Mathf.Sin((Time.time + this.offsetTime) * this.frequency * 2f * 3.1415927f % 6.2831855f) + this.startingIntensity / 2f;
	}

	private float startingIntensity;

	public float frequency;

	private float offsetTime;
}
