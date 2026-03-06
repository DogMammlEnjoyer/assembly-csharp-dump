using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.XR;

[RequireComponent(typeof(RectTransform))]
[ExecuteAlways]
public class OVROverlayCanvas : OVRRayTransformer
{
	private int CanvasRenderLayer
	{
		get
		{
			return OVROverlayCanvasSettings.Instance.CanvasRenderLayer;
		}
	}

	private bool ShouldScaleViewport
	{
		get
		{
			return this._dynamicResolution;
		}
	}

	public bool IsCanvasPriority
	{
		get
		{
			OVROverlayCanvasManager instance = OVROverlayCanvasManager.Instance;
			return ((instance != null) ? new bool?(instance.IsCanvasPriority(this)) : null) ?? false;
		}
	}

	public bool ShouldShowImposter
	{
		get
		{
			return !this.IsCanvasPriority || !this.overlayEnabled || this.overlayType == OVROverlay.OverlayType.Underlay;
		}
	}

	public bool overlayEnabled
	{
		get
		{
			return this._overlayEnabled;
		}
		set
		{
			if (this._overlay && Application.isPlaying)
			{
				this._overlay.enabled = value;
				this._imposterMaterial.color = (value ? Color.black : Color.white);
			}
			this._overlayEnabled = value;
		}
	}

	private void Start()
	{
		if (this.rectTransform == null)
		{
			this.rectTransform = base.GetComponent<RectTransform>();
		}
		HideFlags hideFlags = HideFlags.HideAndDontSave;
		GameObject gameObject = new GameObject(base.name + " Overlay Camera")
		{
			hideFlags = hideFlags
		};
		gameObject.transform.SetParent(base.transform, false);
		this._camera = gameObject.AddComponent<Camera>();
		this._camera.stereoTargetEye = StereoTargetEyeMask.None;
		this._camera.transform.position = base.transform.position - this._camera.transform.forward;
		this._camera.orthographic = true;
		this._camera.enabled = false;
		this._camera.clearFlags = CameraClearFlags.Color;
		this._camera.backgroundColor = Color.clear;
		this._camera.nearClipPlane = 0.99f;
		this._camera.farClipPlane = 1.01f;
		GameObject gameObject2 = new GameObject(base.name + " Imposter")
		{
			hideFlags = hideFlags
		};
		gameObject2.transform.SetParent(base.transform, false);
		gameObject2.AddComponent<MeshFilter>();
		this._meshRenderer = gameObject2.AddComponent<MeshRenderer>();
		this._meshGenerator = gameObject2.AddComponent<OVROverlayMeshGenerator>();
		GameObject gameObject3 = new GameObject(base.name + " Overlay")
		{
			hideFlags = hideFlags
		};
		gameObject3.transform.SetParent(base.transform, false);
		this._overlay = gameObject3.AddComponent<OVROverlay>();
		this._overlay.enabled = false;
		this._overlay.isDynamic = true;
		this.UpdateOverlaySettings();
		this._useTempRT = Application.isMobilePlatform;
		this.InitializeRenderTexture();
	}

	private static string ToSimpleJson<T>(T value)
	{
		OVROverlayCanvas.<>c__DisplayClass50_0<T> CS$<>8__locals1 = new OVROverlayCanvas.<>c__DisplayClass50_0<T>();
		CS$<>8__locals1.value = value;
		OVROverlayCanvas.<>c__DisplayClass50_0<T> CS$<>8__locals2 = CS$<>8__locals1;
		ref T ptr = ref CS$<>8__locals2.value;
		T t = default(T);
		object obj;
		if (t == null)
		{
			t = CS$<>8__locals2.value;
			ptr = ref t;
			if (t == null)
			{
				obj = null;
				goto IL_42;
			}
		}
		obj = ptr.GetType();
		IL_42:
		object obj2 = obj;
		if (obj2 == null || obj2.IsValueType)
		{
			string result;
			if (CS$<>8__locals1.value is bool)
			{
				bool flag = CS$<>8__locals1.value as bool;
				result = (flag ? "true" : "false");
			}
			else if (!(CS$<>8__locals1.value is Enum) && !(CS$<>8__locals1.value is string))
			{
				OVROverlayCanvas.<>c__DisplayClass50_0<T> CS$<>8__locals3 = CS$<>8__locals1;
				ref T ptr2 = ref CS$<>8__locals3.value;
				t = default(T);
				string text;
				if (t == null)
				{
					t = CS$<>8__locals3.value;
					ptr2 = ref t;
					if (t == null)
					{
						text = null;
						goto IL_106;
					}
				}
				text = ptr2.ToString();
				IL_106:
				result = text;
			}
			else
			{
				result = string.Format("\"{0}\"", CS$<>8__locals1.value);
			}
			return result;
		}
		PropertyInfo[] properties = CS$<>8__locals1.value.GetType().GetProperties();
		if (properties.Length == 0)
		{
			return "{}";
		}
		IEnumerable<string> values = from p in properties
		select "\"" + p.Name + "\":" + OVROverlayCanvas.ToSimpleJson<object>(p.GetValue(CS$<>8__locals1.value));
		return "{" + string.Join(",", values) + "}";
	}

	public void UpdateOverlaySettings()
	{
		this.InitializeRenderTexture();
		this._meshRenderer.enabled = this.ShouldShowImposter;
		this._overlay.noDepthBufferTesting = this.ShouldShowImposter;
		this._overlay.isAlphaPremultiplied = true;
		this._overlay.currentOverlayType = this.overlayType;
		this._overlay.enabled = this.overlayEnabled;
	}

	private void InitializeRenderTexture()
	{
		if (this.rectTransform == null)
		{
			this.rectTransform = base.GetComponent<RectTransform>();
		}
		float width = this.rectTransform.rect.width;
		float height = this.rectTransform.rect.height;
		float num = (width >= height) ? 1f : (width / height);
		float num2 = (height >= width) ? 1f : (height / width);
		int num3 = this.ShouldScaleViewport ? 0 : 8;
		int num4 = Mathf.CeilToInt(num * (float)(this.maxTextureSize - num3 * 2));
		int num5 = Mathf.CeilToInt(num2 * (float)(this.maxTextureSize - num3 * 2));
		int num6 = num4 + num3 * 2;
		int num7 = num5 + num3 * 2;
		float x = width * ((float)num6 / (float)num4);
		float num8 = height * ((float)num7 / (float)num5);
		if (this._renderTexture == null || this._renderTexture.width != num6 || this._renderTexture.height != num7)
		{
			if (this._renderTexture != null)
			{
				Object.DestroyImmediate(this._renderTexture);
			}
			RenderTextureDescriptor desc = new RenderTextureDescriptor(num6, num7, GraphicsFormat.R8G8B8A8_SRGB, GraphicsFormat.D24_UNorm_S8_UInt);
			desc.autoGenerateMips = (desc.useMipMap = this._enableMipmapping);
			this._renderTexture = new RenderTexture(desc);
			this._renderTexture.filterMode = FilterMode.Trilinear;
			this._renderTexture.name = base.name;
		}
		this._camera.orthographicSize = 0.5f * num8 * this.GetRectTransformScale().y;
		this._camera.targetTexture = this._renderTexture;
		this._camera.cullingMask = 1 << this.CanvasRenderLayer;
		Shader shader = OVROverlayCanvasSettings.Instance.GetShader(this.opacity);
		if (this._imposterMaterial == null)
		{
			this._imposterMaterial = new Material(shader);
		}
		else
		{
			this._imposterMaterial.shader = shader;
		}
		if (this.opacity == OVROverlayCanvas.DrawMode.OpaqueWithClip)
		{
			this._imposterMaterial.EnableKeyword("WITH_CLIP");
		}
		else
		{
			this._imposterMaterial.DisableKeyword("WITH_CLIP");
		}
		if (this.expensive)
		{
			this._imposterMaterial.EnableKeyword("EXPENSIVE");
		}
		else
		{
			this._imposterMaterial.DisableKeyword("EXPENSIVE");
		}
		if (this.opacity == OVROverlayCanvas.DrawMode.AlphaToMask)
		{
			this._imposterMaterial.EnableKeyword("ALPHA_TO_MASK");
			this._imposterMaterial.SetInt("_AlphaToMask", 1);
		}
		else
		{
			this._imposterMaterial.DisableKeyword("ALPHA_TO_MASK");
			this._imposterMaterial.SetInt("_AlphaToMask", 0);
		}
		if (this.overlayEnabled && this.overlapMask)
		{
			this._imposterMaterial.EnableKeyword("OVERLAP_MASK");
		}
		else
		{
			this._imposterMaterial.DisableKeyword("OVERLAP_MASK");
		}
		this._imposterMaterial.mainTexture = this._renderTexture;
		this._imposterMaterial.color = this.CalcImposterColor();
		this._imposterMaterial.mainTextureOffset = this._imposterTextureOffset;
		this._imposterMaterial.mainTextureScale = this._imposterTextureScale;
		this._meshRenderer.sharedMaterial = this._imposterMaterial;
		this._meshRenderer.gameObject.layer = this.layer;
		if (this.shape == OVROverlayCanvas.CanvasShape.Flat)
		{
			Transform transform = this._meshRenderer.transform;
			Vector3 localPosition = this._overlay.transform.localPosition = Vector3.zero;
			transform.localPosition = localPosition;
			this._meshRenderer.transform.localScale = new Vector3(width, height, 1f);
			this._overlay.transform.localScale = new Vector3(x, num8, 1f);
		}
		else
		{
			Transform transform2 = this._meshRenderer.transform;
			Transform transform3 = this._overlay.transform;
			Vector3 localPosition = new Vector3(0f, 0f, -this.curveRadius / base.transform.lossyScale.z);
			transform3.localPosition = localPosition;
			transform2.localPosition = localPosition;
			this._meshRenderer.transform.localScale = new Vector3(width, height, this.curveRadius / base.transform.lossyScale.z);
			this._overlay.transform.localScale = new Vector3(x, num8, this.curveRadius / base.transform.lossyScale.z);
		}
		this._overlay.textures[0] = this._renderTexture;
		this._overlay.currentOverlayShape = ((this.shape == OVROverlayCanvas.CanvasShape.Flat) ? OVROverlay.OverlayShape.Quad : OVROverlay.OverlayShape.Cylinder);
		this._overlay.hidden = !this.IsCanvasPriority;
		this._overlay.useExpensiveSuperSample = this.expensive;
		this._overlay.enabled = (Application.isPlaying && this._overlayEnabled);
		this._overlay.useAutomaticFiltering = true;
		this._meshGenerator.SetOverlay(this._overlay);
		OVROverlayCanvasSettings.Instance.ApplyGlobalSettings();
	}

	private Color CalcImposterColor()
	{
		if (!this.overlayEnabled || !this.IsCanvasPriority || this.overlayType != OVROverlay.OverlayType.Underlay)
		{
			return Color.white;
		}
		return Color.black;
	}

	private void OnDestroy()
	{
		if (Application.isPlaying)
		{
			Object.Destroy(this._imposterMaterial);
			Object.Destroy(this._renderTexture);
			return;
		}
		Object.DestroyImmediate(this._imposterMaterial);
		Object.DestroyImmediate(this._renderTexture);
	}

	private void OnEnable()
	{
		OVROverlayCanvasManager.AddCanvas(this);
		if (this._overlay)
		{
			this._meshRenderer.enabled = this.ShouldShowImposter;
			this._overlay.enabled = (Application.isPlaying && this._overlayEnabled);
		}
	}

	private void OnDisable()
	{
		OVROverlayCanvasManager.RemoveCanvas(this);
		if (this._overlay)
		{
			this._overlay.enabled = false;
			this._meshRenderer.enabled = false;
		}
	}

	protected virtual bool ShouldRender()
	{
		if (this.manualRedraw && this._frameIsReady)
		{
			if (this._dynamicResolution && this._redrawResolutionThreshold != 2147483647)
			{
				ValueTuple<int, int>? valueTuple = this.CalculateScaledResolution();
				if (valueTuple != null)
				{
					ValueTuple<int, int> valueOrDefault = valueTuple.GetValueOrDefault();
					int item = valueOrDefault.Item1;
					int item2 = valueOrDefault.Item2;
					return item - this._lastPixelWidth >= this._redrawResolutionThreshold || item2 - this._lastPixelHeight >= this._redrawResolutionThreshold;
				}
			}
			return false;
		}
		return (this.renderInterval <= 1 || Time.frameCount % this.renderInterval == this.renderIntervalFrameOffset % this.renderInterval || !this._frameIsReady) && (Application.isEditor || this.IsInFrustum());
	}

	private bool IsInFrustum()
	{
		Camera camera = OVRManager.FindMainCamera();
		if (camera != null)
		{
			XRDisplaySubsystem currentDisplaySubsystem = OVRManager.GetCurrentDisplaySubsystem();
			if (currentDisplaySubsystem != null && currentDisplaySubsystem.GetRenderPassCount() > 0)
			{
				for (int i = 0; i < currentDisplaySubsystem.GetRenderPassCount(); i++)
				{
					XRDisplaySubsystem.XRRenderPass xrrenderPass;
					currentDisplaySubsystem.GetRenderPass(i, out xrrenderPass);
					ScriptableCullingParameters scriptableCullingParameters;
					currentDisplaySubsystem.GetCullingParameters(camera, xrrenderPass.cullingPassIndex, out scriptableCullingParameters);
					GeometryUtility.CalculateFrustumPlanes(scriptableCullingParameters.stereoProjectionMatrix * scriptableCullingParameters.stereoViewMatrix, OVROverlayCanvas._FrustumPlanes);
					if (GeometryUtility.TestPlanesAABB(OVROverlayCanvas._FrustumPlanes, this._meshRenderer.bounds))
					{
						return true;
					}
				}
			}
			else if (camera.stereoEnabled)
			{
				for (int j = 0; j < 2; j++)
				{
					Camera.StereoscopicEye eye = (Camera.StereoscopicEye)j;
					GeometryUtility.CalculateFrustumPlanes(camera.GetStereoProjectionMatrix(eye) * camera.GetStereoViewMatrix(eye), OVROverlayCanvas._FrustumPlanes);
					if (GeometryUtility.TestPlanesAABB(OVROverlayCanvas._FrustumPlanes, this._meshRenderer.bounds))
					{
						return true;
					}
				}
			}
			else
			{
				GeometryUtility.CalculateFrustumPlanes(camera.projectionMatrix * camera.worldToCameraMatrix, OVROverlayCanvas._FrustumPlanes);
				if (GeometryUtility.TestPlanesAABB(OVROverlayCanvas._FrustumPlanes, this._meshRenderer.bounds))
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}

	private void Update()
	{
		this.UpdateOverlaySettings();
		bool flag = this.ShouldRender();
		this._overlay.isDynamic = flag;
		if (!flag)
		{
			return;
		}
		this.ApplyViewportScale();
		this._frameIsReady = true;
		this.RenderCamera();
	}

	private void LateUpdate()
	{
		this._imposterMaterial.color = this.CalcImposterColor();
		this._imposterMaterial.mainTextureScale = this._imposterTextureScale;
		this._imposterMaterial.mainTextureOffset = this._imposterTextureOffset;
	}

	public float? GetViewPriorityScore()
	{
		int renderedFrameCount = Time.renderedFrameCount;
		if (this._lastViewPriorityScore.Item1 != renderedFrameCount)
		{
			this._lastViewPriorityScore = new ValueTuple<int, float?>(renderedFrameCount, this.GetViewPriorityScoreImpl());
		}
		return this._lastViewPriorityScore.Item2;
	}

	private float? GetViewPriorityScoreImpl()
	{
		Camera camera = OVRManager.FindMainCamera();
		if (camera == null)
		{
			return null;
		}
		if (!this._overlayEnabled)
		{
			return null;
		}
		this.rectTransform.GetWorldCorners(OVROverlayCanvas._Corners);
		for (int num = 0; num != 4; num++)
		{
			Vector3 vector = camera.WorldToViewportPoint(OVROverlayCanvas._Corners[num]);
			vector.x = Mathf.Clamp01(vector.x) - 0.5f;
			vector.y = Mathf.Clamp01(vector.y) - 0.5f;
			vector.z = ((vector.z < 0f) ? float.NaN : 0f);
			OVROverlayCanvas._Corners[num] = vector;
		}
		float value = (OVROverlayCanvas.TriangleArea(OVROverlayCanvas._Corners[0], OVROverlayCanvas._Corners[1], OVROverlayCanvas._Corners[2]) + OVROverlayCanvas.TriangleArea(OVROverlayCanvas._Corners[1], OVROverlayCanvas._Corners[2], OVROverlayCanvas._Corners[3])) / Mathf.Max(((OVROverlayCanvas._Corners[0] + OVROverlayCanvas._Corners[1] + OVROverlayCanvas._Corners[2] + OVROverlayCanvas._Corners[3]) * 0.25f).magnitude, 0.01f);
		float f;
		if (!float.IsNaN(f))
		{
			return new float?(value);
		}
		return null;
	}

	private static float TriangleArea(Vector3 a, Vector3 b, Vector3 c)
	{
		return Vector3.Cross(b - a, c - a).magnitude * 0.5f;
	}

	private void OnValidate()
	{
	}

	private Vector3 GetRectTransformScale()
	{
		Vector3 localScale = this.rectTransform.localScale;
		Vector3 vector = (this.rectTransform.parent != null) ? this.rectTransform.parent.lossyScale : Vector3.one;
		if ((!Mathf.Approximately(vector.x, vector.y) || !Mathf.Approximately(vector.y, vector.z)) && !this._nonUniformScaleWarningShown)
		{
			Debug.LogWarning("[OVROverlayCanvas][" + base.name + "] Non Uniform Parent Scale. This will result in unexpected behavior!", this);
			this._nonUniformScaleWarningShown = true;
		}
		return new Vector3(vector.x * localScale.x, vector.y * localScale.y, vector.z * localScale.z);
	}

	private Matrix4x4 GetWorldToViewportMatrix(Camera mainCamera)
	{
		XRDisplaySubsystem currentDisplaySubsystem = OVRManager.GetCurrentDisplaySubsystem();
		if (currentDisplaySubsystem != null && currentDisplaySubsystem.GetRenderPassCount() > 0)
		{
			XRDisplaySubsystem.XRRenderPass xrrenderPass;
			currentDisplaySubsystem.GetRenderPass(0, out xrrenderPass);
			XRDisplaySubsystem.XRRenderParameter xrrenderParameter;
			xrrenderPass.GetRenderParameter(mainCamera, 0, out xrrenderParameter);
			return xrrenderParameter.projection * mainCamera.worldToCameraMatrix;
		}
		return mainCamera.projectionMatrix * mainCamera.worldToCameraMatrix;
	}

	[return: TupleElementNames(new string[]
	{
		"pixelWidth",
		"pixelHeight"
	})]
	private ValueTuple<int, int>? CalculateScaledResolution()
	{
		if (!this.ShouldScaleViewport)
		{
			return new ValueTuple<int, int>?(new ValueTuple<int, int>(this._renderTexture.width, this._renderTexture.height));
		}
		if (!this.IsInFrustum())
		{
			return new ValueTuple<int, int>?(new ValueTuple<int, int>(32, 32));
		}
		Camera camera = OVRManager.FindMainCamera();
		if (camera == null)
		{
			return null;
		}
		if (!this._optimalResolutionInitialized && XRSettings.isDeviceActive)
		{
			this._optimalResolutionWidth = (float)XRSettings.eyeTextureWidth * 2f / XRSettings.eyeTextureResolutionScale;
			this._optimalResolutionHeight = (float)XRSettings.eyeTextureHeight * 2f / XRSettings.eyeTextureResolutionScale;
			this._optimalResolutionInitialized = (this._optimalResolutionWidth > 0f && this._optimalResolutionHeight > 0f);
		}
		this.rectTransform.GetLocalCorners(OVROverlayCanvas._Corners);
		Matrix4x4 matrix4x = this.rectTransform.localToWorldMatrix;
		if (this.shape == OVROverlayCanvas.CanvasShape.Curved)
		{
			matrix4x *= this.CalculateCurveViewBillboardMatrix(camera);
		}
		Matrix4x4 worldToViewportMatrix = this.GetWorldToViewportMatrix(camera);
		Matrix4x4 matrix4x2 = Matrix4x4.Scale(new Vector3(0.5f * this._optimalResolutionWidth, 0.5f * this._optimalResolutionHeight, 0f)) * worldToViewportMatrix * matrix4x;
		for (int i = 0; i < 4; i++)
		{
			OVROverlayCanvas._Corners[i] = matrix4x2.MultiplyPoint(OVROverlayCanvas._Corners[i]);
		}
		int num = Mathf.RoundToInt(Mathf.Max((OVROverlayCanvas._Corners[1] - OVROverlayCanvas._Corners[0]).magnitude, (OVROverlayCanvas._Corners[3] - OVROverlayCanvas._Corners[2]).magnitude));
		int num2 = Mathf.RoundToInt(Mathf.Max((OVROverlayCanvas._Corners[2] - OVROverlayCanvas._Corners[1]).magnitude, (OVROverlayCanvas._Corners[3] - OVROverlayCanvas._Corners[0]).magnitude));
		int num3 = (num + 1) / 2 * 2 * (this.expensive ? 2 : 1) + 4;
		int value = (num2 + 1) / 2 * 2 * (this.expensive ? 2 : 1) + 4;
		num3 = Mathf.Clamp(num3, 32, this._renderTexture.height);
		return new ValueTuple<int, int>?(new ValueTuple<int, int>(Mathf.Clamp(value, 32, this._renderTexture.width), num3));
	}

	private void ApplyViewportScale()
	{
		ValueTuple<int, int>? valueTuple = this.CalculateScaledResolution();
		if (valueTuple != null)
		{
			ValueTuple<int, int> valueOrDefault = valueTuple.GetValueOrDefault();
			int num = valueOrDefault.Item1;
			int num2 = valueOrDefault.Item2;
			if (Math.Abs(num2 - this._lastPixelHeight) < 4 && Math.Abs(num - this._lastPixelWidth) < 4)
			{
				num = this._lastPixelWidth;
				num2 = this._lastPixelHeight;
			}
			else
			{
				this._lastPixelHeight = num2;
				this._lastPixelWidth = num;
			}
			int num3 = num2 - 4;
			int num4 = num - 4;
			Vector3 rectTransformScale = this.GetRectTransformScale();
			float num5 = this.rectTransform.rect.height * rectTransformScale.y * (float)num2 / (float)num3;
			float num6 = this.rectTransform.rect.width * rectTransformScale.x * (float)num / (float)num4;
			this._camera.orthographicSize = 0.5f * num5;
			this._camera.aspect = num6 / num5;
			float num7 = (float)num / (float)this._renderTexture.width;
			float num8 = (float)num2 / (float)this._renderTexture.height;
			float num9 = (float)num4 / (float)this._renderTexture.width;
			float num10 = (float)num3 / (float)this._renderTexture.height;
			this._camera.rect = new Rect((1f - num7) / 2f, (1f - num8) / 2f, num7, num8);
			Rect rect = new Rect(0.5f - 0.5f * num9, 0.5f - 0.5f * num10, num9, num10);
			Rect rect2 = new Rect(0f, 0f, 1f, 1f);
			this._overlay.overrideTextureRectMatrix = true;
			this._overlay.SetSrcDestRects(rect, rect, rect2, rect2);
			int num11 = this.ShouldScaleViewport ? 0 : 8;
			Vector2 b = new Vector2((float)num, (float)num2);
			Vector2 b2 = new Vector2(1f / (float)num, 1f / (float)num2);
			this._imposterTextureOffset = (rect.min * b + Vector2.one * (float)num11) * b2;
			this._imposterTextureScale = (rect.size * b - Vector2.one * (float)num11 * 2f) * b2;
			return;
		}
	}

	private void RenderCamera()
	{
		this._camera.transform.position = base.transform.position - this._camera.transform.forward;
		Rect rect = this._camera.rect;
		int num = (int)(rect.width * (float)this._renderTexture.width);
		int num2 = (int)(rect.height * (float)this._renderTexture.height);
		using (OVROverlayCanvas.ScopedCallback scopedCallback = default(OVROverlayCanvas.ScopedCallback))
		{
			if (GraphicsSettings.defaultRenderPipeline == null)
			{
				this._camera.cullingMask = 1 << this.CanvasRenderLayer;
				int targetLayer = base.gameObject.layer;
				Transform[] transforms = base.GetComponentsInChildren<Transform>();
				foreach (Transform transform in transforms)
				{
					if (transform.gameObject.layer == targetLayer)
					{
						transform.gameObject.layer = this.CanvasRenderLayer;
					}
				}
				scopedCallback.OnDispose += delegate()
				{
					foreach (Transform transform2 in transforms)
					{
						if (transform2.gameObject.layer == this.CanvasRenderLayer)
						{
							transform2.gameObject.layer = targetLayer;
						}
					}
				};
			}
			else
			{
				this._camera.cullingMask = 1 << base.gameObject.layer;
			}
			if (this._useTempRT && (num < this._renderTexture.width || num2 < this._renderTexture.height))
			{
				RenderTexture temporary = RenderTexture.GetTemporary(new RenderTextureDescriptor(num, num2, GraphicsFormat.R8G8B8A8_SRGB, GraphicsFormat.D24_UNorm_S8_UInt, 0));
				temporary.Create();
				this._camera.targetTexture = temporary;
				this._camera.rect = new Rect(0f, 0f, 1f, 1f);
				this._camera.Render();
				Graphics.CopyTexture(temporary, 0, 0, 0, 0, num, num2, this._renderTexture, 0, 0, (int)(rect.x * (float)this._renderTexture.width), (int)(rect.y * (float)this._renderTexture.height));
				RenderTexture.ReleaseTemporary(temporary);
				this._camera.rect = rect;
				this._camera.targetTexture = this._renderTexture;
			}
			else
			{
				this._camera.Render();
			}
		}
	}

	private Matrix4x4 CalculateCurveViewBillboardMatrix(Camera mainCamera)
	{
		Vector3 vector = Quaternion.Inverse(this.rectTransform.rotation) * (mainCamera.transform.position - this.rectTransform.position);
		float num = Mathf.Atan2(-vector.x, -vector.z);
		Vector3 rectTransformScale = this.GetRectTransformScale();
		float num2 = this.rectTransform.rect.width * rectTransformScale.x / this.curveRadius;
		num = Mathf.Clamp(num, -0.5f * num2, 0.5f * num2);
		Vector3 b = new Vector3(num * this.curveRadius, 0f, 0f);
		Vector3 a = new Vector3(0f, 0f, this.curveRadius);
		return Matrix4x4.Scale(new Vector3(1f / rectTransformScale.x, 1f / rectTransformScale.y, 1f / rectTransformScale.z)) * Matrix4x4.Translate(-a) * Matrix4x4.Rotate(Quaternion.AngleAxis(57.29578f * num, Vector3.up)) * Matrix4x4.Translate(a - b) * Matrix4x4.Scale(new Vector3(rectTransformScale.x, rectTransformScale.y, 1f));
	}

	public override Ray TransformRay(Ray ray)
	{
		if (this.shape != OVROverlayCanvas.CanvasShape.Curved)
		{
			return ray;
		}
		Vector3 vector = base.transform.InverseTransformPoint(ray.origin);
		Vector3 vector2 = base.transform.InverseTransformDirection(ray.direction);
		float num = this.curveRadius / base.transform.lossyScale.z;
		Vector3 vector3 = new Vector3(0f, 0f, -num);
		float d;
		if (!OVROverlayCanvas.LineCircleIntersection(new Vector2(vector.x, vector.z), new Vector2(vector2.x, vector2.z), new Vector2(vector3.x, vector3.z), num, out d))
		{
			return new Ray(ray.origin, base.transform.right);
		}
		Vector3 vector4 = vector + vector2 * d;
		float x = Mathf.Atan2(vector4.x, vector4.z + num) * num;
		float y = vector4.y;
		return new Ray(base.transform.TransformPoint(new Vector3(x, y, -1f)), base.transform.forward);
	}

	private static bool LineCircleIntersection(Vector2 p1, Vector2 dp, Vector2 center, float radius, out float distance)
	{
		float sqrMagnitude = dp.sqrMagnitude;
		float num = 2f * Vector2.Dot(dp, p1 - center);
		float num2 = center.sqrMagnitude;
		num2 += p1.sqrMagnitude;
		num2 -= 2f * Vector2.Dot(center, p1);
		num2 -= radius * radius;
		float num3 = num * num - 4f * sqrMagnitude * num2;
		if (Mathf.Abs(sqrMagnitude) < 1E-45f || num3 < 0f)
		{
			distance = 0f;
			return false;
		}
		float num4 = (-num - Mathf.Sqrt(num3)) / (2f * sqrMagnitude);
		float num5 = (-num + Mathf.Sqrt(num3)) / (2f * sqrMagnitude);
		distance = ((num4 >= 0f) ? num4 : num5);
		return true;
	}

	public OVROverlay Overlay
	{
		get
		{
			return this._overlay;
		}
	}

	public void SetFrameDirty()
	{
		this._frameIsReady = false;
	}

	public void SetCanvasLayer(int layer, bool forceUpdate)
	{
		OVROverlayCanvas.SetLayerRecursive(base.gameObject, layer, base.gameObject.layer, forceUpdate);
	}

	private static void SetLayerRecursive(GameObject gameObject, int layer, int previousLayer, bool forceUpdate)
	{
		if (gameObject.layer == previousLayer || forceUpdate)
		{
			gameObject.layer = layer;
		}
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			GameObject gameObject2 = gameObject.transform.GetChild(i).gameObject;
			if ((gameObject2.hideFlags &= HideFlags.DontSave) == HideFlags.None)
			{
				OVROverlayCanvas.SetLayerRecursive(gameObject2, layer, previousLayer, forceUpdate);
			}
		}
	}

	private const float kOptimalResolutionScale = 2f;

	private Camera _camera;

	private OVROverlay _overlay;

	private MeshRenderer _meshRenderer;

	private OVROverlayMeshGenerator _meshGenerator;

	private RenderTexture _renderTexture;

	private Material _imposterMaterial;

	private bool _optimalResolutionInitialized;

	private float _optimalResolutionWidth;

	private float _optimalResolutionHeight;

	private int _lastPixelWidth;

	private int _lastPixelHeight;

	private Vector2 _imposterTextureOffset;

	private Vector2 _imposterTextureScale;

	private bool _frameIsReady;

	private bool _useTempRT;

	[SerializeField]
	internal bool _enableMipmapping;

	[SerializeField]
	internal bool _dynamicResolution = true;

	[SerializeField]
	internal int _redrawResolutionThreshold = int.MaxValue;

	public RectTransform rectTransform;

	[FormerlySerializedAs("MaxTextureSize")]
	public int maxTextureSize = 2048;

	public bool manualRedraw;

	[FormerlySerializedAs("DrawRate")]
	public int renderInterval = 1;

	[FormerlySerializedAs("DrawFrameOffset")]
	public int renderIntervalFrameOffset;

	[FormerlySerializedAs("Expensive")]
	public bool expensive;

	[FormerlySerializedAs("Layer")]
	public int layer = 5;

	[FormerlySerializedAs("Opacity")]
	public OVROverlayCanvas.DrawMode opacity = OVROverlayCanvas.DrawMode.Transparent;

	public OVROverlayCanvas.CanvasShape shape;

	public float curveRadius = 1f;

	public bool overlapMask;

	public OVROverlay.OverlayType overlayType = OVROverlay.OverlayType.Underlay;

	[SerializeField]
	internal bool _overlayEnabled = true;

	private static readonly Plane[] _FrustumPlanes = new Plane[6];

	private static readonly Vector3[] _Corners = new Vector3[4];

	private bool _nonUniformScaleWarningShown;

	[TupleElementNames(new string[]
	{
		"frameCount",
		"score"
	})]
	private ValueTuple<int, float?> _lastViewPriorityScore = new ValueTuple<int, float?>(-1, null);

	public enum DrawMode
	{
		Opaque,
		OpaqueWithClip,
		Transparent,
		[Obsolete("Deprecated. Use Transparent", false)]
		TransparentDefaultAlpha = 2,
		[Obsolete("Deprecated. Use Transparent", false)]
		TransparentCorrectAlpha,
		AlphaToMask
	}

	public enum CanvasShape
	{
		Flat,
		Curved
	}

	public struct ScopedCallback : IDisposable
	{
		public event Action OnDispose;

		void IDisposable.Dispose()
		{
			Action onDispose = this.OnDispose;
			if (onDispose == null)
			{
				return;
			}
			onDispose();
		}
	}
}
