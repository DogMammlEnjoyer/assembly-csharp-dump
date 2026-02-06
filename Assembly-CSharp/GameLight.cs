using System;
using UnityEngine;

public class GameLight : MonoBehaviour
{
	public float InitialIntensity { get; private set; }

	public void Awake()
	{
		this.intensityMult = 1;
	}

	protected void OnEnable()
	{
		if (this.initialized)
		{
			this.lightId = GameLightingManager.instance.AddGameLight(this, false);
		}
	}

	protected void Start()
	{
		this.lightId = GameLightingManager.instance.AddGameLight(this, false);
		this.initialized = true;
	}

	protected void OnDisable()
	{
		if (this.initialized)
		{
			GameLightingManager.instance.RemoveGameLight(this);
		}
	}

	public Light light;

	public bool negativeLight;

	public bool isHighPriorityPlayerLight;

	public Vector3 cachedPosition;

	public Vector4 cachedColorAndIntensity;

	public int lightId;

	public int intensityMult = 1;

	private bool initialized;
}
