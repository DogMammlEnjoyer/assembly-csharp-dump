using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class GameLightingManager : MonoBehaviourTick, IGorillaSliceableSimple
{
	public bool IsDynamicLightingEnabled
	{
		get
		{
			return this.customVertexLightingEnabled;
		}
	}

	private static uint PackHalf2(float a, float b)
	{
		return (uint)((int)Mathf.FloatToHalf(a) | (int)Mathf.FloatToHalf(b) << 16);
	}

	private void Awake()
	{
		this.InitData();
	}

	private void InitData()
	{
		GameLightingManager.instance = this;
		this.gameLights = new List<GameLight>(512);
		this.sortKeys = new float[512];
		this.sortValues = new GameLight[512];
		this.lightDataBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 50, UnsafeUtility.SizeOf<GameLightingManager.LightDataPacked>());
		this.lightData = new NativeArray<GameLightingManager.LightDataPacked>(50, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.nextLightUpdate = 0;
		this.ClearGameLights();
		this.SetDesaturateAndTintEnabled(false, Color.black);
		this.SetAmbientLightDynamic(Color.black);
		this.SetCustomDynamicLightingEnabled(false);
		this.SetMaxLights(20);
	}

	private void OnDestroy()
	{
		this.ClearGameLights();
		this.SetDesaturateAndTintEnabled(false, Color.black);
		this.SetAmbientLightDynamic(Color.black);
		this.SetCustomDynamicLightingEnabled(false);
		GraphicsBuffer graphicsBuffer = this.lightDataBuffer;
		if (graphicsBuffer != null)
		{
			graphicsBuffer.Dispose();
		}
		if (this.lightData.IsCreated)
		{
			this.lightData.Dispose();
		}
	}

	public new void OnEnable()
	{
		base.OnEnable();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	public new void OnDisable()
	{
		base.OnDisable();
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	public void ZoneEnableCustomDynamicLighting(bool enable)
	{
		if (enable)
		{
			if (this.zoneDynamicLightingEnableCount == 0)
			{
				this.SetCustomDynamicLightingEnabled(true);
			}
			this.zoneDynamicLightingEnableCount++;
			return;
		}
		this.zoneDynamicLightingEnableCount--;
		if (this.zoneDynamicLightingEnableCount == 0)
		{
			this.SetCustomDynamicLightingEnabled(false);
		}
		if (this.zoneDynamicLightingEnableCount < 0)
		{
			Debug.LogErrorFormat("Zone Dynamic Lighting Ref count is {0} and should never be less that 0", new object[]
			{
				this.zoneDynamicLightingEnableCount
			});
			this.zoneDynamicLightingEnableCount = 0;
		}
	}

	public void SetCustomDynamicLightingEnabled(bool enable)
	{
		this.customVertexLightingEnabled = enable;
		if (this.customVertexLightingEnabled)
		{
			Shader.EnableKeyword("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
			return;
		}
		Shader.DisableKeyword("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
	}

	public void ToggleCustomDynamicLightingEnabled()
	{
		this.SetCustomDynamicLightingEnabled(!this.customVertexLightingEnabled);
	}

	public void SetAmbientLightDynamic(Color color)
	{
		Shader.SetGlobalColor(GameLightingManager._shaderPropId_GameLight_Ambient_Color, color);
	}

	public void SetMaxLights(int maxLights)
	{
		maxLights = Mathf.Min(maxLights, 50);
		this.maxUseTestLights = maxLights;
		Shader.SetGlobalInteger(GameLightingManager._shaderPropId_GameLight_UseMaxLights, maxLights);
	}

	public void SetDesaturateAndTintEnabled(bool enable, Color tint)
	{
		Shader.SetGlobalColor(GameLightingManager._shaderPropId_DesaturateAndTint_TintColor, tint);
		Shader.SetGlobalFloat(GameLightingManager._shaderPropId_DesaturateAndTint_TintAmount, enable ? 1f : 0f);
		this.desaturateAndTintEnabled = enable;
	}

	public void SliceUpdate()
	{
		if (this.skipNextSlice)
		{
			this.skipNextSlice = false;
			return;
		}
		this.immediateSort = false;
		this.SortLights();
	}

	public void SortLights()
	{
		int count = this.gameLights.Count;
		if (count <= this.maxUseTestLights)
		{
			return;
		}
		if (this.mainCameraTransform == null)
		{
			this.mainCameraTransform = Camera.main.transform;
		}
		Vector3 position = this.mainCameraTransform.position;
		if (this.sortKeys == null || this.sortKeys.Length < count)
		{
			int num = Mathf.Max(count, (this.sortKeys != null) ? (this.sortKeys.Length * 2) : 64);
			this.sortKeys = new float[num];
			this.sortValues = new GameLight[num];
		}
		for (int i = 0; i < count; i++)
		{
			GameLight gameLight = this.gameLights[i];
			if (gameLight == null || gameLight.light == null)
			{
				this.sortKeys[i] = float.MaxValue;
			}
			else
			{
				float num2 = Mathf.Clamp(gameLight.cachedColorAndIntensity.x + gameLight.cachedColorAndIntensity.y + gameLight.cachedColorAndIntensity.z, 0.01f, 6f);
				Vector3 vector = position - gameLight.cachedPosition;
				this.sortKeys[i] = (vector.x * vector.x + vector.y * vector.y + vector.z * vector.z) / num2;
			}
			this.sortValues[i] = gameLight;
		}
		Array.Sort<float, GameLight>(this.sortKeys, this.sortValues, 0, count);
		for (int j = 0; j < count; j++)
		{
			this.gameLights[j] = this.sortValues[j];
		}
	}

	public override void Tick()
	{
		this.RefreshLightData();
	}

	private void RefreshLightData()
	{
		if (this.lightDataBuffer == null)
		{
			return;
		}
		if (this.customVertexLightingEnabled)
		{
			int numLightsToPull = 10;
			if (this.immediateSort)
			{
				this.immediateSort = false;
				this.skipNextSlice = true;
				this.CacheAllLightData();
				this.SortLights();
				numLightsToPull = this.maxUseTestLights;
			}
			else
			{
				int numLightsToUpdateCache = 5;
				this.CacheLightDataForNonCloseLights(numLightsToUpdateCache);
			}
			this.PullLightData(numLightsToPull);
			int num = Mathf.Min(this.gameLights.Count, this.maxUseTestLights);
			this.lightDataBuffer.SetData<GameLightingManager.LightDataPacked>(this.lightData, 0, 0, num);
			Shader.SetGlobalBuffer(GameLightingManager._shaderPropId_GameLight_Lights, this.lightDataBuffer);
			Shader.SetGlobalInteger(GameLightingManager._shaderPropId_GameLight_UseMaxLights, num);
		}
	}

	public void CacheAllLightData()
	{
		for (int i = 0; i < this.gameLights.Count; i++)
		{
			GameLight gameLight = this.gameLights[i];
			if (gameLight != null && gameLight.light != null)
			{
				gameLight.cachedPosition = gameLight.transform.position;
				gameLight.cachedColorAndIntensity = (float)gameLight.intensityMult * gameLight.light.intensity * (gameLight.negativeLight ? -1f : 1f) * gameLight.light.color;
			}
		}
	}

	public void CacheLightDataForNonCloseLights(int numLightsToUpdateCache)
	{
		int num = this.gameLights.Count - this.maxUseTestLights;
		if (num <= 0)
		{
			return;
		}
		for (int i = 0; i < numLightsToUpdateCache; i++)
		{
			this.nextLightCacheUpdate = (this.nextLightCacheUpdate + 1) % num;
			GameLight gameLight = this.gameLights[this.maxUseTestLights + this.nextLightCacheUpdate];
			if (gameLight != null && gameLight.light != null)
			{
				gameLight.cachedPosition = gameLight.transform.position;
				gameLight.cachedColorAndIntensity = (float)gameLight.intensityMult * gameLight.light.intensity * (gameLight.negativeLight ? -1f : 1f) * gameLight.light.color;
			}
		}
	}

	public void PullLightData(int numLightsToPull)
	{
		for (int i = 0; i < this.maxUseTestLights; i++)
		{
			if (i < this.gameLights.Count && this.gameLights[i] != null && this.gameLights[i].isHighPriorityPlayerLight)
			{
				this.GetFromLight(i, i);
			}
		}
		for (int j = 0; j < numLightsToPull; j++)
		{
			this.nextLightUpdate = (this.nextLightUpdate + 1) % this.maxUseTestLights;
			if (this.nextLightUpdate < this.gameLights.Count)
			{
				this.GetFromLight(this.nextLightUpdate, this.nextLightUpdate);
				if (this.gameLights[this.nextLightUpdate] != null && this.gameLights[this.nextLightUpdate].isHighPriorityPlayerLight)
				{
				}
			}
			else
			{
				this.ResetLight(this.nextLightUpdate);
			}
		}
	}

	public int AddGameLight(GameLight light, bool ignoreUnityLightDisable = false)
	{
		if (light == null || !light.gameObject.activeInHierarchy || light.light == null || !light.light.enabled)
		{
			return -1;
		}
		if (light.IsRegistered)
		{
			return -1;
		}
		if (!ignoreUnityLightDisable)
		{
			light.light.enabled = false;
		}
		this.gameLights.Add(light);
		this.immediateSort = true;
		return this.gameLights.Count - 1;
	}

	public void RemoveGameLight(GameLight light)
	{
		if (light != null && light.light != null)
		{
			light.light.enabled = true;
		}
		if (light != null)
		{
			light.lightId = -1;
		}
		int num = this.gameLights.IndexOf(light);
		if (num >= 0)
		{
			this.gameLights.RemoveAt(num);
		}
	}

	public void ClearGameLights()
	{
		if (this.gameLights != null)
		{
			this.gameLights.Clear();
		}
		if (this.lightDataBuffer == null)
		{
			return;
		}
		for (int i = 0; i < 50; i++)
		{
			this.ResetLight(i);
		}
		this.lightDataBuffer.SetData<GameLightingManager.LightDataPacked>(this.lightData);
		Shader.SetGlobalBuffer(GameLightingManager._shaderPropId_GameLight_Lights, this.lightDataBuffer);
	}

	public void GetFromLight(int lightIndex, int gameLightIndex)
	{
		if (this.lightDataBuffer == null)
		{
			return;
		}
		GameLight gameLight = null;
		if (gameLightIndex >= 0 && gameLightIndex < this.gameLights.Count)
		{
			gameLight = this.gameLights[gameLightIndex];
		}
		if (gameLight == null || gameLight.light == null)
		{
			return;
		}
		gameLight.cachedPosition = gameLight.transform.position;
		gameLight.cachedColorAndIntensity = (float)gameLight.intensityMult * gameLight.light.intensity * (gameLight.negativeLight ? -1f : 1f) * gameLight.light.color;
		Vector3 cachedPosition = gameLight.cachedPosition;
		Vector4 cachedColorAndIntensity = gameLight.cachedColorAndIntensity;
		this.lightData[lightIndex] = new GameLightingManager.LightDataPacked
		{
			posXY = GameLightingManager.PackHalf2(cachedPosition.x, cachedPosition.y),
			posZW = GameLightingManager.PackHalf2(cachedPosition.z, 1f),
			colorRG = GameLightingManager.PackHalf2(cachedColorAndIntensity.x, cachedColorAndIntensity.y),
			colorBA = GameLightingManager.PackHalf2(cachedColorAndIntensity.z, cachedColorAndIntensity.w)
		};
	}

	private void ResetLight(int lightIndex)
	{
		this.lightData[lightIndex] = default(GameLightingManager.LightDataPacked);
	}

	public Light GR_NearsightedDimLight
	{
		get
		{
			return this._GR_NearsightedDimLight;
		}
	}

	[OnEnterPlay_SetNull]
	public static volatile GameLightingManager instance;

	public const int MAX_VERTEX_LIGHTS = 50;

	public const int USE_MAX_VERTEX_LIGHTS = 20;

	public const int MAX_UPDATE_LIGHTS_PER_FRAME = 10;

	public Transform testLightsCenter;

	[ColorUsage(true, true)]
	public Color testAmbience = Color.black;

	[ColorUsage(true, true)]
	public Color testLightColor = Color.white;

	public float testLightBrightness = 10f;

	public float testLightRadius = 2f;

	public int maxUseTestLights = 1;

	[ReadOnly]
	[SerializeField]
	private List<GameLight> gameLights;

	private bool customVertexLightingEnabled;

	private bool desaturateAndTintEnabled;

	private Transform mainCameraTransform;

	private int zoneDynamicLightingEnableCount;

	private float[] sortKeys;

	private GameLight[] sortValues;

	private NativeArray<GameLightingManager.LightDataPacked> lightData;

	private GraphicsBuffer lightDataBuffer;

	private bool skipNextSlice;

	private bool immediateSort;

	private int nextLightUpdate;

	private int nextLightCacheUpdate;

	[SerializeField]
	private Light _GR_NearsightedDimLight;

	private static readonly int _shaderPropId_GameLight_UseMaxLights = Shader.PropertyToID("_GT_GameLight_UseMaxLights");

	private static readonly int _shaderPropId_DesaturateAndTint_TintColor = Shader.PropertyToID("_GT_DesaturateAndTint_TintColor");

	private static readonly int _shaderPropId_DesaturateAndTint_TintAmount = Shader.PropertyToID("_GT_DesaturateAndTint_TintAmount");

	private static readonly int _shaderPropId_GameLight_Ambient_Color = Shader.PropertyToID("_GT_GameLight_Ambient_Color");

	private static readonly int _shaderPropId_GameLight_Lights = Shader.PropertyToID("_GT_GameLight_Lights");

	private struct LightInput
	{
		public Color color;

		public float intensity;

		public float intensityMult;
	}

	private struct LightDataPacked
	{
		public uint posXY;

		public uint posZW;

		public uint colorRG;

		public uint colorBA;
	}
}
