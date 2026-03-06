using System;
using UnityEngine;
using UnityEngine.Rendering;

public class OVROverlayCanvasSettings : OVRRuntimeAssetsBase
{
	public static OVROverlayCanvasSettings Instance
	{
		get
		{
			if (OVROverlayCanvasSettings._instance == null)
			{
				OVROverlayCanvasSettings._instance = OVROverlayCanvasSettings.GetOverlayCanvasSettings();
			}
			return OVROverlayCanvasSettings._instance;
		}
	}

	private static OVROverlayCanvasSettings GetOverlayCanvasSettings()
	{
		OVROverlayCanvasSettings ovroverlayCanvasSettings;
		OVRRuntimeAssetsBase.LoadAsset<OVROverlayCanvasSettings>(out ovroverlayCanvasSettings, "OVROverlayCanvasSettings", null);
		if (ovroverlayCanvasSettings == null)
		{
			Debug.LogWarning("Failed to load runtime settings. Using default runtime settings instead.");
			ovroverlayCanvasSettings = ScriptableObject.CreateInstance<OVROverlayCanvasSettings>();
		}
		ovroverlayCanvasSettings.EnsureInitialized();
		return ovroverlayCanvasSettings;
	}

	public void ApplyGlobalSettings()
	{
	}

	public Shader GetShader(OVROverlayCanvas.DrawMode drawMode)
	{
		switch (drawMode)
		{
		case OVROverlayCanvas.DrawMode.Opaque:
		case OVROverlayCanvas.DrawMode.OpaqueWithClip:
		case OVROverlayCanvas.DrawMode.AlphaToMask:
			return this._opaqueImposterShader;
		}
		return this._transparentImposterShader;
	}

	private static bool UsingBuiltInRenderPipeline()
	{
		return GraphicsSettings.currentRenderPipeline == null;
	}

	private static void EnsureShaderInitialized(ref Shader shader, string shaderName, string replaceShaderName)
	{
		if (shader != null && shader.name != replaceShaderName)
		{
			return;
		}
		Shader shader2 = Shader.Find(shaderName);
		if (shader2 == null)
		{
			Debug.LogError("Failed to find shader \"" + shaderName + "\"");
			return;
		}
		shader = shader2;
	}

	private void EnsureInitialized()
	{
		bool flag = OVROverlayCanvasSettings.UsingBuiltInRenderPipeline();
		OVROverlayCanvasSettings.EnsureShaderInitialized(ref this._opaqueImposterShader, flag ? "UI/Prerendered Opaque" : "URP/UI/Prerendered Opaque", flag ? "URP/UI/Prerendered Opaque" : "UI/Prerendered Opaque");
		OVROverlayCanvasSettings.EnsureShaderInitialized(ref this._transparentImposterShader, flag ? "UI/Prerendered" : "URP/UI/Prerendered", flag ? "URP/UI/Prerendered" : "UI/Prerendered");
	}

	private void OnValidate()
	{
		this.EnsureInitialized();
	}

	private const string kAssetName = "OVROverlayCanvasSettings";

	private const string kBuiltInOpaqueShaderName = "UI/Prerendered Opaque";

	private const string kUrpOpaqueShaderName = "URP/UI/Prerendered Opaque";

	private const string kBuiltInTransparentShaderName = "UI/Prerendered";

	private const string kUrpTransparentShaderName = "URP/UI/Prerendered";

	private static OVROverlayCanvasSettings _instance;

	[SerializeField]
	private Shader _transparentImposterShader;

	[SerializeField]
	private Shader _opaqueImposterShader;

	public int MaxSimultaneousCanvases = 1;

	public int CanvasRenderLayer = 31;

	public int CanvasLayer = -1;
}
