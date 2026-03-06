using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.Events;

[HelpURL("https://developer.oculus.com/documentation/unity/unity-passthrough-gs/")]
[Feature(Feature.Passthrough)]
public class OVRPassthroughLayer : MonoBehaviour
{
	public void AddSurfaceGeometry(GameObject obj, bool updateTransform = false)
	{
		if (this.projectionSurfaceType != OVRPassthroughLayer.ProjectionSurfaceType.UserDefined)
		{
			Debug.LogError("Passthrough layer is not configured for surface projected passthrough.");
			return;
		}
		if (this.surfaceGameObjects.ContainsKey(obj))
		{
			Debug.LogError("Specified GameObject has already been added as passthrough surface.");
			return;
		}
		if (obj.GetComponent<MeshFilter>() == null)
		{
			Debug.LogError("Specified GameObject does not have a mesh component.");
			return;
		}
		this.deferredSurfaceGameObjects.Add(new OVRPassthroughLayer.DeferredPassthroughMeshAddition
		{
			gameObject = obj,
			updateTransform = updateTransform
		});
	}

	public void RemoveSurfaceGeometry(GameObject obj)
	{
		OVRPassthroughLayer.PassthroughMeshInstance passthroughMeshInstance;
		if (!this.surfaceGameObjects.TryGetValue(obj, out passthroughMeshInstance))
		{
			if (this.deferredSurfaceGameObjects.RemoveAll((OVRPassthroughLayer.DeferredPassthroughMeshAddition x) => x.gameObject == obj) == 0)
			{
				Debug.LogError("Specified GameObject has not been added as passthrough surface.");
			}
			return;
		}
		if (OVRPlugin.DestroyInsightPassthroughGeometryInstance(passthroughMeshInstance.instanceHandle) && OVRPlugin.DestroyInsightTriangleMesh(passthroughMeshInstance.meshHandle))
		{
			this.surfaceGameObjects.Remove(obj);
			return;
		}
		Debug.LogError("GameObject could not be removed from passthrough surface.");
	}

	public bool IsSurfaceGeometry(GameObject obj)
	{
		return this.surfaceGameObjects.ContainsKey(obj) || this.deferredSurfaceGameObjects.Exists((OVRPassthroughLayer.DeferredPassthroughMeshAddition x) => x.gameObject == obj);
	}

	public float textureOpacity
	{
		get
		{
			return this.textureOpacity_;
		}
		set
		{
			value = Mathf.Clamp01(value);
			if (value != this.textureOpacity_)
			{
				this.textureOpacity_ = value;
				this.styleDirty = true;
			}
		}
	}

	public bool edgeRenderingEnabled
	{
		get
		{
			return this.edgeRenderingEnabled_;
		}
		set
		{
			if (value != this.edgeRenderingEnabled_)
			{
				this.edgeRenderingEnabled_ = value;
				this.styleDirty = true;
			}
		}
	}

	public Color edgeColor
	{
		get
		{
			return this.edgeColor_;
		}
		set
		{
			if (value != this.edgeColor_)
			{
				this.edgeColor_ = value;
				this.styleDirty = true;
			}
		}
	}

	[Obsolete("This event is deprecated, use passthroughLayerResumed UnityEvent instead", false)]
	public event Action PassthroughLayerResumed;

	public void SetColorMap(Color[] values)
	{
		if (values.Length != 256)
		{
			throw new ArgumentException("Must provide exactly 256 colors");
		}
		this.colorMapType = OVRPlugin.InsightPassthroughColorMapType.MonoToRgba;
		this.colorMapEditorType = OVRPassthroughLayer.ColorMapEditorType.Custom;
		this._stylesHandler.SetMonoToRgbaHandler(values);
		this.styleDirty = true;
	}

	public void SetColorLut(OVRPassthroughColorLut lut, float weight = 1f)
	{
		if (lut != null && lut.IsValid)
		{
			weight = OVRPassthroughLayer.ClampWeight(weight);
			this.colorMapType = OVRPlugin.InsightPassthroughColorMapType.ColorLut;
			this.colorMapEditorType = OVRPassthroughLayer.ColorMapEditorType.Custom;
			this._stylesHandler.SetColorLutHandler(lut, weight);
			this.styleDirty = true;
			return;
		}
		Debug.LogError("Trying to set an invalid Color LUT for Passthrough");
	}

	public void SetColorLut(OVRPassthroughColorLut lutSource, OVRPassthroughColorLut lutTarget, float weight)
	{
		if (lutSource != null && lutSource.IsValid && lutTarget != null && lutTarget.IsValid)
		{
			weight = OVRPassthroughLayer.ClampWeight(weight);
			this.colorMapType = OVRPlugin.InsightPassthroughColorMapType.InterpolatedColorLut;
			this.colorMapEditorType = OVRPassthroughLayer.ColorMapEditorType.Custom;
			this._stylesHandler.SetInterpolatedColorLutHandler(lutSource, lutTarget, weight);
			this.styleDirty = true;
			return;
		}
		Debug.LogError("Trying to set an invalid Color LUT for Passthrough");
	}

	public void SetColorMapControls(float contrast, float brightness = 0f, float posterize = 0f, Gradient gradient = null, OVRPassthroughLayer.ColorMapEditorType colorMapType = OVRPassthroughLayer.ColorMapEditorType.GrayscaleToColor)
	{
		if (colorMapType != OVRPassthroughLayer.ColorMapEditorType.Grayscale && colorMapType != OVRPassthroughLayer.ColorMapEditorType.GrayscaleToColor)
		{
			Debug.LogError("Unsupported color map type specified");
			return;
		}
		this.colorMapEditorType = colorMapType;
		this.colorMapEditorContrast = contrast;
		this.colorMapEditorBrightness = brightness;
		this.colorMapEditorPosterize = posterize;
		if (colorMapType == OVRPassthroughLayer.ColorMapEditorType.GrayscaleToColor)
		{
			if (gradient != null)
			{
				this.colorMapEditorGradient = gradient;
				return;
			}
			if (!this.colorMapEditorGradient.Equals(OVRPassthroughLayer.colorMapNeutralGradient))
			{
				this.colorMapEditorGradient = OVRPassthroughLayer.CreateNeutralColorMapGradient();
				return;
			}
		}
		else if (gradient != null)
		{
			Debug.LogWarning("Gradient parameter is ignored for color map types other than GrayscaleToColor");
		}
	}

	public void SetColorMapMonochromatic(byte[] values)
	{
		if (values.Length != 256)
		{
			throw new ArgumentException("Must provide exactly 256 values");
		}
		this.colorMapType = OVRPlugin.InsightPassthroughColorMapType.MonoToMono;
		this.colorMapEditorType = OVRPassthroughLayer.ColorMapEditorType.Custom;
		this._stylesHandler.SetMonoToMonoHandler(values);
		this.styleDirty = true;
	}

	public void SetBrightnessContrastSaturation(float brightness = 0f, float contrast = 0f, float saturation = 0f)
	{
		this.colorMapType = OVRPlugin.InsightPassthroughColorMapType.BrightnessContrastSaturation;
		this.colorMapEditorType = OVRPassthroughLayer.ColorMapEditorType.ColorAdjustment;
		this.colorMapEditorBrightness = brightness;
		this.colorMapEditorContrast = contrast;
		this.colorMapEditorSaturation = saturation;
		this.UpdateColorMapFromControls(false);
	}

	public void DisableColorMap()
	{
		this.colorMapEditorType = OVRPassthroughLayer.ColorMapEditorType.None;
	}

	public OVRPassthroughLayer.ColorMapEditorType colorMapEditorType
	{
		get
		{
			return this.colorMapEditorType_;
		}
		set
		{
			if (value != this.colorMapEditorType_)
			{
				this.colorMapEditorType_ = value;
				if (value != OVRPassthroughLayer.ColorMapEditorType.Custom)
				{
					this.colorMapType = OVRPassthroughLayer._editorToColorMapType[value];
					this._stylesHandler.SetStyleHandler(this.colorMapType);
					if (value == OVRPassthroughLayer.ColorMapEditorType.None)
					{
						this.styleDirty = true;
						return;
					}
					this.UpdateColorMapFromControls(true);
				}
			}
		}
	}

	public void SetStyleDirty()
	{
		this.styleDirty = true;
	}

	private void AddDeferredSurfaceGeometries()
	{
		for (int i = 0; i < this.deferredSurfaceGameObjects.Count; i++)
		{
			OVRPassthroughLayer.DeferredPassthroughMeshAddition deferredPassthroughMeshAddition = this.deferredSurfaceGameObjects[i];
			bool flag = false;
			if (deferredPassthroughMeshAddition.gameObject)
			{
				ulong meshHandle;
				ulong instanceHandle;
				Matrix4x4 localToWorld;
				if (this.surfaceGameObjects.ContainsKey(deferredPassthroughMeshAddition.gameObject))
				{
					flag = true;
				}
				else if (this.CreateAndAddMesh(deferredPassthroughMeshAddition.gameObject, out meshHandle, out instanceHandle, out localToWorld))
				{
					this.surfaceGameObjects.Add(deferredPassthroughMeshAddition.gameObject, new OVRPassthroughLayer.PassthroughMeshInstance
					{
						meshHandle = meshHandle,
						instanceHandle = instanceHandle,
						updateTransform = deferredPassthroughMeshAddition.updateTransform,
						localToWorld = localToWorld
					});
					flag = true;
				}
				else
				{
					Debug.LogWarning("Failed to create internal resources for GameObject added to passthrough surface.");
				}
			}
			if (flag)
			{
				this.deferredSurfaceGameObjects.RemoveAt(i--);
			}
		}
	}

	private Matrix4x4 GetTransformMatrixForPassthroughSurfaceObject(Matrix4x4 worldFromObj)
	{
		Matrix4x4 result;
		using (new OVRProfilerScope("GetTransformMatrixForPassthroughSurfaceObject"))
		{
			if (!this.cameraRigInitialized)
			{
				this.cameraRig = OVRManager.instance.GetComponentInParent<OVRCameraRig>();
				this.cameraRigInitialized = true;
			}
			Matrix4x4 rhs = (this.cameraRig != null) ? this.cameraRig.trackingSpace.worldToLocalMatrix : Matrix4x4.identity;
			result = Matrix4x4.Scale(new Vector3(1f, 1f, -1f)) * rhs * worldFromObj;
		}
		return result;
	}

	private bool CreateAndAddMesh(GameObject obj, out ulong meshHandle, out ulong instanceHandle, out Matrix4x4 localToWorld)
	{
		meshHandle = 0UL;
		instanceHandle = 0UL;
		localToWorld = obj.transform.localToWorldMatrix;
		MeshFilter component = obj.GetComponent<MeshFilter>();
		if (component == null)
		{
			Debug.LogError("Passthrough surface GameObject does not have a mesh component.");
			return false;
		}
		Mesh sharedMesh = component.sharedMesh;
		Vector3[] vertices = sharedMesh.vertices;
		int[] triangles = sharedMesh.triangles;
		Matrix4x4 transformMatrixForPassthroughSurfaceObject = this.GetTransformMatrixForPassthroughSurfaceObject(localToWorld);
		if (!OVRPlugin.CreateInsightTriangleMesh(this.passthroughOverlay.layerId, vertices, triangles, out meshHandle))
		{
			Debug.LogWarning("Failed to create triangle mesh handle.");
			return false;
		}
		if (!OVRPlugin.AddInsightPassthroughSurfaceGeometry(this.passthroughOverlay.layerId, meshHandle, transformMatrixForPassthroughSurfaceObject, out instanceHandle))
		{
			Debug.LogWarning("Failed to add mesh to passthrough surface.");
			return false;
		}
		return true;
	}

	private void DestroySurfaceGeometries(bool addBackToDeferredQueue = false)
	{
		foreach (KeyValuePair<GameObject, OVRPassthroughLayer.PassthroughMeshInstance> keyValuePair in this.surfaceGameObjects)
		{
			if (keyValuePair.Value.meshHandle != 0UL)
			{
				OVRPlugin.DestroyInsightPassthroughGeometryInstance(keyValuePair.Value.instanceHandle);
				OVRPlugin.DestroyInsightTriangleMesh(keyValuePair.Value.meshHandle);
				if (addBackToDeferredQueue)
				{
					this.deferredSurfaceGameObjects.Add(new OVRPassthroughLayer.DeferredPassthroughMeshAddition
					{
						gameObject = keyValuePair.Key,
						updateTransform = keyValuePair.Value.updateTransform
					});
				}
			}
		}
		this.surfaceGameObjects.Clear();
	}

	private void UpdateSurfaceGeometryTransforms()
	{
		using (new OVRProfilerScope("UpdateSurfaceGeometryTransforms"))
		{
			List<GameObject> list;
			using (new OVRObjectPool.ListScope<GameObject>(ref list))
			{
				foreach (KeyValuePair<GameObject, OVRPassthroughLayer.PassthroughMeshInstance> keyValuePair in this.surfaceGameObjects)
				{
					if (keyValuePair.Key == null)
					{
						list.Add(keyValuePair.Key);
					}
					else
					{
						ulong instanceHandle = keyValuePair.Value.instanceHandle;
						if (instanceHandle != 0UL)
						{
							Matrix4x4 localToWorld = keyValuePair.Value.updateTransform ? keyValuePair.Key.transform.localToWorldMatrix : keyValuePair.Value.localToWorld;
							this.UpdateSurfaceGeometryTransform(instanceHandle, localToWorld);
						}
					}
				}
				foreach (GameObject obj in list)
				{
					this.RemoveSurfaceGeometry(obj);
				}
			}
		}
	}

	private void UpdateSurfaceGeometryTransform(ulong instanceHandle, Matrix4x4 localToWorld)
	{
		Matrix4x4 transformMatrixForPassthroughSurfaceObject = this.GetTransformMatrixForPassthroughSurfaceObject(localToWorld);
		using (new OVRProfilerScope("UpdateInsightPassthroughGeometryTransform"))
		{
			if (!OVRPlugin.UpdateInsightPassthroughGeometryTransform(instanceHandle, transformMatrixForPassthroughSurfaceObject))
			{
				Debug.LogWarning("Failed to update a transform of a passthrough surface");
			}
		}
	}

	internal static Gradient CreateNeutralColorMapGradient()
	{
		return new Gradient
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(new Color(0f, 0f, 0f), 0f),
				new GradientColorKey(new Color(1f, 1f, 1f), 1f)
			},
			alphaKeys = new GradientAlphaKey[]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			}
		};
	}

	private bool HasControlsBasedColorMap()
	{
		return this.colorMapEditorType == OVRPassthroughLayer.ColorMapEditorType.Grayscale || this.colorMapEditorType == OVRPassthroughLayer.ColorMapEditorType.ColorAdjustment || this.colorMapEditorType == OVRPassthroughLayer.ColorMapEditorType.ColorLut || this.colorMapEditorType == OVRPassthroughLayer.ColorMapEditorType.InterpolatedColorLut || this.colorMapEditorType == OVRPassthroughLayer.ColorMapEditorType.GrayscaleToColor;
	}

	private void UpdateColorMapFromControls(bool forceUpdate = false)
	{
		bool flag = this._settings.brightness != this.colorMapEditorBrightness || this._settings.contrast != this.colorMapEditorContrast || this._settings.posterize != this.colorMapEditorPosterize || this._settings.colorLutSourceTexture != this._colorLutSourceTexture || this._settings.colorLutTargetTexture != this._colorLutTargetTexture || this._settings.lutWeight != this._lutWeight || this._settings.saturation != this.colorMapEditorSaturation || this._settings.flipLutY != this._flipLutY;
		bool flag2 = this.colorMapEditorType == OVRPassthroughLayer.ColorMapEditorType.GrayscaleToColor && !this.colorMapEditorGradient.Equals(this._settings.gradient);
		if ((!this.HasControlsBasedColorMap() || !flag) && !flag2 && !forceUpdate)
		{
			return;
		}
		this._settings.gradient.CopyFrom(this.colorMapEditorGradient);
		this._settings.brightness = this.colorMapEditorBrightness;
		this._settings.contrast = this.colorMapEditorContrast;
		this._settings.posterize = this.colorMapEditorPosterize;
		this._settings.saturation = this.colorMapEditorSaturation;
		this._settings.lutWeight = this._lutWeight;
		this._settings.flipLutY = this._flipLutY;
		this._settings.colorLutSourceTexture = this._colorLutSourceTexture;
		this._settings.colorLutTargetTexture = this._colorLutTargetTexture;
		if (Application.isPlaying)
		{
			this._stylesHandler.CurrentStyleHandler.Update(this._settings);
			this.styleDirty = true;
		}
	}

	private void SyncToOverlay()
	{
		this.passthroughOverlay.currentOverlayType = this.overlayType;
		this.passthroughOverlay.compositionDepth = this.compositionDepth;
		this.passthroughOverlay.hidden = (this.hidden || this.IsUserDefinedAndDoesNotContainSurfaceGeometry());
		this.passthroughOverlay.overridePerLayerColorScaleAndOffset = this.overridePerLayerColorScaleAndOffset;
		this.passthroughOverlay.colorScale = this.colorScale;
		this.passthroughOverlay.colorOffset = this.colorOffset;
		if (this.passthroughOverlay.currentOverlayShape != this.overlayShape)
		{
			if (this.passthroughOverlay.layerId > 0)
			{
				Debug.LogWarning("Change to projectionSurfaceType won't take effect until the layer goes through a disable/enable cycle. ");
			}
			if (this.projectionSurfaceType == OVRPassthroughLayer.ProjectionSurfaceType.Reconstructed)
			{
				Debug.Log("Removing user defined surface geometries");
				this.DestroySurfaceGeometries(false);
			}
			this.passthroughOverlay.currentOverlayShape = this.overlayShape;
		}
		bool enabled = this.passthroughOverlay.enabled;
		this.passthroughOverlay.enabled = (OVRManager.instance != null && OVRManager.instance.isInsightPassthroughEnabled && OVRManager.IsInsightPassthroughInitialized());
		if (enabled != this.passthroughOverlay.enabled)
		{
			if (this.passthroughOverlay.enabled)
			{
				this.styleDirty = true;
				return;
			}
			this.DestroySurfaceGeometries(true);
		}
	}

	private bool IsUserDefinedAndDoesNotContainSurfaceGeometry()
	{
		return this.projectionSurfaceType == OVRPassthroughLayer.ProjectionSurfaceType.UserDefined && this.deferredSurfaceGameObjects.Count == 0 && this.surfaceGameObjects.Count == 0;
	}

	private static float ClampWeight(float weight)
	{
		if (weight < 0f || weight > 1f)
		{
			Debug.LogWarning("Color lut weight should be between in [0, 1] range. Setting it to closest value.");
			weight = Mathf.Clamp01(weight);
		}
		return weight;
	}

	private OVROverlay.OverlayShape overlayShape
	{
		get
		{
			if (this.projectionSurfaceType != OVRPassthroughLayer.ProjectionSurfaceType.UserDefined)
			{
				return OVROverlay.OverlayShape.ReconstructionPassthrough;
			}
			return OVROverlay.OverlayShape.SurfaceProjectedPassthrough;
		}
	}

	private void Awake()
	{
		foreach (OVRPassthroughLayer.SerializedSurfaceGeometry serializedSurfaceGeometry in this.serializedSurfaceGeometry)
		{
			if (!(serializedSurfaceGeometry.meshFilter == null))
			{
				this.deferredSurfaceGameObjects.Add(new OVRPassthroughLayer.DeferredPassthroughMeshAddition
				{
					gameObject = serializedSurfaceGeometry.meshFilter.gameObject,
					updateTransform = serializedSurfaceGeometry.updateTransform
				});
			}
		}
	}

	private void Update()
	{
		this.SyncToOverlay();
	}

	private void LateUpdate()
	{
		if (this.hidden)
		{
			return;
		}
		if (this.passthroughOverlay.layerId <= 0)
		{
			return;
		}
		if (this.projectionSurfaceType == OVRPassthroughLayer.ProjectionSurfaceType.UserDefined)
		{
			this.UpdateSurfaceGeometryTransforms();
			this.AddDeferredSurfaceGeometries();
		}
		this.UpdateColorMapFromControls(false);
		if (this.styleDirty)
		{
			if (this._stylesHandler.CurrentStyleHandler.IsValid)
			{
				OVRPlugin.SetInsightPassthroughStyle(this.passthroughOverlay.layerId, this.CreateOvrPluginStyleObject());
			}
			this.styleDirty = false;
		}
	}

	private OVRPlugin.InsightPassthroughStyle2 CreateOvrPluginStyleObject()
	{
		OVRPlugin.InsightPassthroughStyle2 result = default(OVRPlugin.InsightPassthroughStyle2);
		result.Flags = (OVRPlugin.InsightPassthroughStyleFlags)7;
		result.TextureOpacityFactor = this.textureOpacity;
		result.EdgeColor = (this.edgeRenderingEnabled ? this.edgeColor.ToColorf() : new OVRPlugin.Colorf
		{
			r = 0f,
			g = 0f,
			b = 0f,
			a = 0f
		});
		result.TextureColorMapType = this.colorMapType;
		result.TextureColorMapData = IntPtr.Zero;
		result.TextureColorMapDataSize = 0U;
		this._stylesHandler.CurrentStyleHandler.ApplyStyleSettings(ref result);
		return result;
	}

	private void OnEnable()
	{
		this.auxGameObject = new GameObject("OVRPassthroughLayer auxiliary GameObject");
		this.auxGameObject.transform.parent = base.transform;
		this.passthroughOverlay = this.auxGameObject.AddComponent<OVROverlay>();
		this.passthroughOverlay.currentOverlayShape = this.overlayShape;
		OVRManager.PassthroughLayerResumed += this.OnPassthroughLayerResumed;
		this.SyncToOverlay();
		if (this.colorMapEditorType != OVRPassthroughLayer.ColorMapEditorType.Custom)
		{
			this._stylesHandler.SetStyleHandler(OVRPassthroughLayer._editorToColorMapType[this.colorMapEditorType]);
		}
		if (this.HasControlsBasedColorMap())
		{
			this.UpdateColorMapFromControls(true);
		}
		this.styleDirty = true;
	}

	private void OnDisable()
	{
		OVRManager.PassthroughLayerResumed -= this.OnPassthroughLayerResumed;
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
		{
			this.DestroySurfaceGeometries(true);
		}
		if (this.auxGameObject != null)
		{
			Object.Destroy(this.auxGameObject);
			this.auxGameObject = null;
			this.passthroughOverlay = null;
		}
	}

	private void OnDestroy()
	{
		this.DestroySurfaceGeometries(false);
	}

	private void OnPassthroughLayerResumed(int layerId)
	{
		if (this.passthroughOverlay != null && this.passthroughOverlay.layerId == layerId)
		{
			if (this.PassthroughLayerResumed != null)
			{
				this.PassthroughLayerResumed();
			}
			UnityEvent<OVRPassthroughLayer> unityEvent = this.passthroughLayerResumed;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}
	}

	public OVRPassthroughLayer.ProjectionSurfaceType projectionSurfaceType;

	public OVROverlay.OverlayType overlayType = OVROverlay.OverlayType.Overlay;

	public int compositionDepth;

	public bool hidden;

	public bool overridePerLayerColorScaleAndOffset;

	public Vector4 colorScale = Vector4.one;

	public Vector4 colorOffset = Vector4.zero;

	public UnityEvent<OVRPassthroughLayer> passthroughLayerResumed = new UnityEvent<OVRPassthroughLayer>();

	[SerializeField]
	internal OVRPassthroughLayer.ColorMapEditorType colorMapEditorType_;

	private static Dictionary<OVRPassthroughLayer.ColorMapEditorType, OVRPlugin.InsightPassthroughColorMapType> _editorToColorMapType = new Dictionary<OVRPassthroughLayer.ColorMapEditorType, OVRPlugin.InsightPassthroughColorMapType>
	{
		{
			OVRPassthroughLayer.ColorMapEditorType.None,
			OVRPlugin.InsightPassthroughColorMapType.None
		},
		{
			OVRPassthroughLayer.ColorMapEditorType.Grayscale,
			OVRPlugin.InsightPassthroughColorMapType.MonoToMono
		},
		{
			OVRPassthroughLayer.ColorMapEditorType.GrayscaleToColor,
			OVRPlugin.InsightPassthroughColorMapType.MonoToRgba
		},
		{
			OVRPassthroughLayer.ColorMapEditorType.ColorAdjustment,
			OVRPlugin.InsightPassthroughColorMapType.BrightnessContrastSaturation
		},
		{
			OVRPassthroughLayer.ColorMapEditorType.ColorLut,
			OVRPlugin.InsightPassthroughColorMapType.ColorLut
		},
		{
			OVRPassthroughLayer.ColorMapEditorType.InterpolatedColorLut,
			OVRPlugin.InsightPassthroughColorMapType.InterpolatedColorLut
		}
	};

	public Gradient colorMapEditorGradient = OVRPassthroughLayer.CreateNeutralColorMapGradient();

	[Range(-1f, 1f)]
	public float colorMapEditorContrast;

	[Range(-1f, 1f)]
	public float colorMapEditorBrightness;

	[Range(0f, 1f)]
	public float colorMapEditorPosterize;

	[Range(-1f, 1f)]
	public float colorMapEditorSaturation;

	[SerializeField]
	internal Texture2D _colorLutSourceTexture;

	[SerializeField]
	internal Texture2D _colorLutTargetTexture;

	[SerializeField]
	[Range(0f, 1f)]
	internal float _lutWeight = 1f;

	[SerializeField]
	internal bool _flipLutY = true;

	private OVRPassthroughLayer.Settings _settings = new OVRPassthroughLayer.Settings(null, null, 0f, 0f, 0f, 0f, new Gradient(), 1f, true);

	private OVRCameraRig cameraRig;

	private bool cameraRigInitialized;

	private GameObject auxGameObject;

	private OVROverlay passthroughOverlay;

	private Dictionary<GameObject, OVRPassthroughLayer.PassthroughMeshInstance> surfaceGameObjects = new Dictionary<GameObject, OVRPassthroughLayer.PassthroughMeshInstance>();

	private List<OVRPassthroughLayer.DeferredPassthroughMeshAddition> deferredSurfaceGameObjects = new List<OVRPassthroughLayer.DeferredPassthroughMeshAddition>();

	[SerializeField]
	[HideInInspector]
	internal List<OVRPassthroughLayer.SerializedSurfaceGeometry> serializedSurfaceGeometry = new List<OVRPassthroughLayer.SerializedSurfaceGeometry>();

	[SerializeField]
	[Range(0f, 1f)]
	internal float textureOpacity_ = 1f;

	[SerializeField]
	internal bool edgeRenderingEnabled_;

	[SerializeField]
	internal Color edgeColor_ = new Color(1f, 1f, 1f, 1f);

	[SerializeField]
	private OVRPlugin.InsightPassthroughColorMapType colorMapType;

	private bool styleDirty = true;

	private OVRPassthroughLayer.StylesHandler _stylesHandler = new OVRPassthroughLayer.StylesHandler();

	private static readonly Gradient colorMapNeutralGradient = OVRPassthroughLayer.CreateNeutralColorMapGradient();

	public enum ProjectionSurfaceType
	{
		Reconstructed,
		UserDefined
	}

	public enum ColorMapEditorType
	{
		None,
		GrayscaleToColor,
		Controls = 1,
		Custom,
		Grayscale,
		ColorAdjustment,
		ColorLut,
		InterpolatedColorLut
	}

	private struct Settings
	{
		public Settings(Texture2D colorLutTargetTexture, Texture2D colorLutSourceTexture, float saturation, float posterize, float brightness, float contrast, Gradient gradient, float lutWeight, bool flipLutY)
		{
			this.colorLutTargetTexture = colorLutTargetTexture;
			this.colorLutSourceTexture = colorLutSourceTexture;
			this.saturation = saturation;
			this.posterize = posterize;
			this.brightness = brightness;
			this.contrast = contrast;
			this.gradient = gradient;
			this.lutWeight = lutWeight;
			this.flipLutY = flipLutY;
		}

		public Texture2D colorLutTargetTexture;

		public Texture2D colorLutSourceTexture;

		public float saturation;

		public float posterize;

		public float brightness;

		public float contrast;

		public Gradient gradient;

		public float lutWeight;

		public bool flipLutY;
	}

	private struct PassthroughMeshInstance
	{
		public ulong meshHandle;

		public ulong instanceHandle;

		public bool updateTransform;

		public Matrix4x4 localToWorld;
	}

	[Serializable]
	internal struct SerializedSurfaceGeometry
	{
		public MeshFilter meshFilter;

		public bool updateTransform;
	}

	private struct DeferredPassthroughMeshAddition
	{
		public GameObject gameObject;

		public bool updateTransform;
	}

	private interface IStyleHandler
	{
		void ApplyStyleSettings(ref OVRPlugin.InsightPassthroughStyle2 style);

		void Update(OVRPassthroughLayer.Settings settings);

		bool IsValid { get; }

		void Clear();
	}

	private class StylesHandler
	{
		public StylesHandler()
		{
			this._noneHandler = new OVRPassthroughLayer.NoneStyleHandler();
			this._lutHandler = new OVRPassthroughLayer.ColorLutHandler();
			this._interpolatedLutHandler = new OVRPassthroughLayer.InterpolatedColorLutHandler();
			this._monoToMonoHandler = new OVRPassthroughLayer.MonoToMonoStyleHandler(ref this._colorMapDataHandle, this._colorMapData);
			this._monoToRgbaHandler = new OVRPassthroughLayer.MonoToRgbaStyleHandler(ref this._colorMapDataHandle, this._colorMapData);
			this._bcsHandler = new OVRPassthroughLayer.BCSStyleHandler(ref this._colorMapDataHandle, this._colorMapData);
		}

		public void SetStyleHandler(OVRPlugin.InsightPassthroughColorMapType type)
		{
			OVRPassthroughLayer.IStyleHandler styleHandler = this.GetStyleHandler(type);
			if (styleHandler == this.CurrentStyleHandler)
			{
				return;
			}
			if (this.CurrentStyleHandler != null)
			{
				this.CurrentStyleHandler.Clear();
			}
			this.CurrentStyleHandler = styleHandler;
		}

		private OVRPassthroughLayer.IStyleHandler GetStyleHandler(OVRPlugin.InsightPassthroughColorMapType type)
		{
			switch (type)
			{
			case OVRPlugin.InsightPassthroughColorMapType.None:
				return this._noneHandler;
			case OVRPlugin.InsightPassthroughColorMapType.MonoToRgba:
				return this._monoToRgbaHandler;
			case OVRPlugin.InsightPassthroughColorMapType.MonoToMono:
				return this._monoToMonoHandler;
			case OVRPlugin.InsightPassthroughColorMapType.BrightnessContrastSaturation:
				return this._bcsHandler;
			case OVRPlugin.InsightPassthroughColorMapType.ColorLut:
				return this._lutHandler;
			case OVRPlugin.InsightPassthroughColorMapType.InterpolatedColorLut:
				return this._interpolatedLutHandler;
			}
			throw new ArgumentException(string.Format("Unrecognized color map type {0}.", type));
		}

		public void SetColorLutHandler(OVRPassthroughColorLut lut, float weight)
		{
			this.SetStyleHandler(OVRPlugin.InsightPassthroughColorMapType.ColorLut);
			this._lutHandler.Update(lut, weight);
		}

		internal void SetInterpolatedColorLutHandler(OVRPassthroughColorLut lutSource, OVRPassthroughColorLut lutTarget, float weight)
		{
			this.SetStyleHandler(OVRPlugin.InsightPassthroughColorMapType.InterpolatedColorLut);
			this._interpolatedLutHandler.Update(lutSource, lutTarget, weight);
		}

		internal void SetMonoToRgbaHandler(Color[] values)
		{
			this.SetStyleHandler(OVRPlugin.InsightPassthroughColorMapType.MonoToRgba);
			this._monoToRgbaHandler.Update(values);
		}

		internal void SetMonoToMonoHandler(byte[] values)
		{
			this.SetStyleHandler(OVRPlugin.InsightPassthroughColorMapType.MonoToMono);
			this._monoToMonoHandler.Update(values);
		}

		private OVRPassthroughLayer.NoneStyleHandler _noneHandler;

		private OVRPassthroughLayer.ColorLutHandler _lutHandler;

		private OVRPassthroughLayer.InterpolatedColorLutHandler _interpolatedLutHandler;

		private OVRPassthroughLayer.MonoToRgbaStyleHandler _monoToRgbaHandler;

		private OVRPassthroughLayer.MonoToMonoStyleHandler _monoToMonoHandler;

		private OVRPassthroughLayer.BCSStyleHandler _bcsHandler;

		private GCHandle _colorMapDataHandle;

		private byte[] _colorMapData;

		public OVRPassthroughLayer.IStyleHandler CurrentStyleHandler;
	}

	private class NoneStyleHandler : OVRPassthroughLayer.IStyleHandler
	{
		public bool IsValid
		{
			get
			{
				return true;
			}
		}

		public void ApplyStyleSettings(ref OVRPlugin.InsightPassthroughStyle2 style)
		{
		}

		public void Update(OVRPassthroughLayer.Settings settings)
		{
		}

		public void Clear()
		{
		}
	}

	private abstract class BaseGeneratedStyleHandler : OVRPassthroughLayer.IStyleHandler
	{
		protected abstract uint MapSize { get; }

		public bool IsValid
		{
			get
			{
				return true;
			}
		}

		public BaseGeneratedStyleHandler(ref GCHandle colorMapDataHandler, byte[] colorMapData)
		{
			this._colorMapDataHandle = colorMapDataHandler;
			this._colorMapData = colorMapData;
		}

		public virtual void Update(OVRPassthroughLayer.Settings settings)
		{
		}

		public virtual void ApplyStyleSettings(ref OVRPlugin.InsightPassthroughStyle2 style)
		{
			style.TextureColorMapData = this._colorMapDataHandle.AddrOfPinnedObject();
			style.TextureColorMapDataSize = this.MapSize;
			style.TextureColorMapData = this._colorMapDataHandle.AddrOfPinnedObject();
			style.TextureColorMapDataSize = this.MapSize;
		}

		public void Clear()
		{
			this.DeallocateColorMapData();
		}

		protected virtual void AllocateColorMapData(uint size = 4096U)
		{
			if (this._colorMapData != null && (ulong)size != (ulong)((long)this._colorMapData.Length))
			{
				this.DeallocateColorMapData();
			}
			if (this._colorMapData == null)
			{
				this._colorMapData = new byte[size];
				this._colorMapDataHandle = GCHandle.Alloc(this._colorMapData, GCHandleType.Pinned);
			}
		}

		protected virtual void DeallocateColorMapData()
		{
			if (this._colorMapData != null)
			{
				this._colorMapDataHandle.Free();
				this._colorMapData = null;
			}
		}

		protected void WriteColorToColorMap(int colorIndex, ref Color color)
		{
			for (int i = 0; i < 4; i++)
			{
				Buffer.BlockCopy(BitConverter.GetBytes(color[i]), 0, this._colorMapData, colorIndex * 16 + i * 4, 4);
			}
		}

		protected void WriteFloatToColorMap(int index, float value)
		{
			Buffer.BlockCopy(BitConverter.GetBytes(value), 0, this._colorMapData, index * 4, 4);
		}

		protected static void ComputeBrightnessContrastPosterizeMap(byte[] result, float brightness, float contrast, float posterize)
		{
			for (int i = 0; i < 256; i++)
			{
				float num = (float)i / 255f;
				float num2 = contrast + 1f;
				num = (num - 0.5f) * num2 + 0.5f + brightness;
				if (posterize > 0f)
				{
					float num3 = (Mathf.Pow(50f, posterize) - 1f) / 49f;
					num = Mathf.Round(num / num3) * num3;
				}
				result[i] = (byte)(Mathf.Min(Mathf.Max(num, 0f), 1f) * 255f);
			}
		}

		private GCHandle _colorMapDataHandle;

		protected byte[] _colorMapData;
	}

	private class MonoToRgbaStyleHandler : OVRPassthroughLayer.BaseGeneratedStyleHandler
	{
		protected override uint MapSize
		{
			get
			{
				return 4096U;
			}
		}

		public MonoToRgbaStyleHandler(ref GCHandle colorMapDataHandler, byte[] colorMapData) : base(ref colorMapDataHandler, colorMapData)
		{
		}

		public override void Update(OVRPassthroughLayer.Settings settings)
		{
			this.AllocateColorMapData(4096U);
			OVRPassthroughLayer.BaseGeneratedStyleHandler.ComputeBrightnessContrastPosterizeMap(this._tmpColorMapData, settings.brightness, settings.contrast, settings.posterize);
			for (int i = 0; i < 256; i++)
			{
				Color color = settings.gradient.Evaluate((float)this._tmpColorMapData[i] / 255f);
				base.WriteColorToColorMap(i, ref color);
			}
		}

		public void Update(Color[] values)
		{
			this.AllocateColorMapData(4096U);
			for (int i = 0; i < 256; i++)
			{
				base.WriteColorToColorMap(i, ref values[i]);
			}
		}

		protected override void AllocateColorMapData(uint size = 4096U)
		{
			base.AllocateColorMapData(size);
			this._tmpColorMapData = new byte[256];
		}

		protected override void DeallocateColorMapData()
		{
			base.DeallocateColorMapData();
			this._tmpColorMapData = null;
		}

		protected byte[] _tmpColorMapData;
	}

	private class MonoToMonoStyleHandler : OVRPassthroughLayer.BaseGeneratedStyleHandler
	{
		protected override uint MapSize
		{
			get
			{
				return 256U;
			}
		}

		public MonoToMonoStyleHandler(ref GCHandle colorMapDataHandler, byte[] colorMapData) : base(ref colorMapDataHandler, colorMapData)
		{
		}

		public override void Update(OVRPassthroughLayer.Settings settings)
		{
			this.AllocateColorMapData(4096U);
			OVRPassthroughLayer.BaseGeneratedStyleHandler.ComputeBrightnessContrastPosterizeMap(this._colorMapData, settings.brightness, settings.contrast, settings.posterize);
		}

		public void Update(byte[] values)
		{
			this.AllocateColorMapData(4096U);
			Buffer.BlockCopy(values, 0, this._colorMapData, 0, 256);
		}
	}

	private class BCSStyleHandler : OVRPassthroughLayer.BaseGeneratedStyleHandler
	{
		protected override uint MapSize
		{
			get
			{
				return 12U;
			}
		}

		public BCSStyleHandler(ref GCHandle colorMapDataHandler, byte[] colorMapData) : base(ref colorMapDataHandler, colorMapData)
		{
		}

		public override void Update(OVRPassthroughLayer.Settings settings)
		{
			this.AllocateColorMapData(4096U);
			base.WriteFloatToColorMap(0, settings.brightness * 100f);
			base.WriteFloatToColorMap(1, settings.contrast + 1f);
			base.WriteFloatToColorMap(2, settings.saturation + 1f);
		}
	}

	private class ColorLutHandler : OVRPassthroughLayer.IStyleHandler
	{
		public OVRPassthroughColorLut Lut { get; set; }

		public float Weight { get; set; }

		public bool IsValid { get; protected set; }

		public virtual void ApplyStyleSettings(ref OVRPlugin.InsightPassthroughStyle2 style)
		{
			style.LutSource = this.Lut._colorLutHandle;
			style.LutWeight = this.Weight;
		}

		public virtual void Update(OVRPassthroughLayer.Settings settings)
		{
			this.Update(this.GetColorLutForTexture(settings.colorLutSourceTexture, this.Lut, ref this._currentColorLutSourceTexture, settings.flipLutY), settings.lutWeight);
		}

		protected OVRPassthroughColorLut GetColorLutForTexture(Texture2D newTexture, OVRPassthroughColorLut lut, ref Texture2D lastTexture, bool flipY)
		{
			if (newTexture == null)
			{
				Debug.LogError("Trying to update style with null texture.");
				return null;
			}
			if (lastTexture != newTexture || this._currentFlipLutY != flipY)
			{
				if (lut != null)
				{
					lut.Dispose();
				}
				lastTexture = newTexture;
				this._currentFlipLutY = flipY;
				return new OVRPassthroughColorLut(newTexture, this._currentFlipLutY);
			}
			return lut;
		}

		internal void Update(OVRPassthroughColorLut lut, float weight)
		{
			if (lut == null)
			{
				this.IsValid = false;
				return;
			}
			this.IsValid = true;
			this.Lut = lut;
			this.Weight = weight;
		}

		public virtual void Clear()
		{
			this.Lut = null;
			this._currentColorLutSourceTexture = null;
		}

		protected bool _currentFlipLutY;

		protected Texture2D _currentColorLutSourceTexture;
	}

	private class InterpolatedColorLutHandler : OVRPassthroughLayer.ColorLutHandler
	{
		public OVRPassthroughColorLut LutTarget { get; set; }

		public override void ApplyStyleSettings(ref OVRPlugin.InsightPassthroughStyle2 style)
		{
			base.ApplyStyleSettings(ref style);
			style.LutTarget = this.LutTarget._colorLutHandle;
		}

		public override void Update(OVRPassthroughLayer.Settings settings)
		{
			this.Update(base.GetColorLutForTexture(settings.colorLutSourceTexture, base.Lut, ref this._currentColorLutSourceTexture, settings.flipLutY), base.GetColorLutForTexture(settings.colorLutTargetTexture, this.LutTarget, ref this._currentColorLutTargetTexture, settings.flipLutY), settings.lutWeight);
		}

		public void Update(OVRPassthroughColorLut lutSource, OVRPassthroughColorLut lutTarget, float weight)
		{
			if (lutSource == null || lutTarget == null)
			{
				base.IsValid = false;
				return;
			}
			base.IsValid = true;
			base.Lut = lutSource;
			this.LutTarget = lutTarget;
			base.Weight = weight;
		}

		public override void Clear()
		{
			base.Clear();
			this.LutTarget = null;
			this._currentColorLutTargetTexture = null;
		}

		private Texture2D _currentColorLutTargetTexture;
	}
}
