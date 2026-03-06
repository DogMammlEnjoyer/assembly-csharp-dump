using System;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
[ExecuteAlways]
[HelpURL("https://developer.oculus.com/reference/unity/latest/class_o_v_r_vignette")]
public class OVRVignette : MonoBehaviour
{
	private int GetTriangleCount()
	{
		switch (this.MeshComplexity)
		{
		case OVRVignette.MeshComplexityLevel.VerySimple:
			return 32;
		case OVRVignette.MeshComplexityLevel.Simple:
			return 64;
		case OVRVignette.MeshComplexityLevel.Normal:
			return 128;
		case OVRVignette.MeshComplexityLevel.Detailed:
			return 256;
		case OVRVignette.MeshComplexityLevel.VeryDetailed:
			return 512;
		default:
			return 128;
		}
	}

	private void BuildMeshes()
	{
		int triangleCount = this.GetTriangleCount();
		Vector3[] array = new Vector3[triangleCount];
		Vector2[] array2 = new Vector2[triangleCount];
		Vector3[] array3 = new Vector3[triangleCount];
		Vector2[] array4 = new Vector2[triangleCount];
		int[] array5 = new int[triangleCount * 3];
		for (int i = 0; i < triangleCount; i += 2)
		{
			float f = (float)(2 * i) * 3.1415927f / (float)triangleCount;
			float x = Mathf.Cos(f);
			float y = Mathf.Sin(f);
			array3[i] = new Vector3(x, y, 0f);
			array3[i + 1] = new Vector3(x, y, 0f);
			array4[i] = new Vector2(0f, 1f);
			array4[i + 1] = new Vector2(1f, 1f);
			array[i] = new Vector3(x, y, 0f);
			array[i + 1] = new Vector3(x, y, 0f);
			array2[i] = new Vector2(0f, 1f);
			array2[i + 1] = new Vector2(1f, 0f);
			int num = i * 3;
			array5[num] = i;
			array5[num + 1] = i + 1;
			array5[num + 2] = (i + 2) % triangleCount;
			array5[num + 3] = i + 1;
			array5[num + 4] = (i + 3) % triangleCount;
			array5[num + 5] = (i + 2) % triangleCount;
		}
		if (this._OpaqueMesh != null)
		{
			Object.DestroyImmediate(this._OpaqueMesh);
		}
		if (this._TransparentMesh != null)
		{
			Object.DestroyImmediate(this._TransparentMesh);
		}
		this._OpaqueMesh = new Mesh
		{
			name = "Opaque Vignette Mesh",
			hideFlags = HideFlags.HideAndDontSave
		};
		this._TransparentMesh = new Mesh
		{
			name = "Transparent Vignette Mesh",
			hideFlags = HideFlags.HideAndDontSave
		};
		this._OpaqueMesh.vertices = array3;
		this._OpaqueMesh.uv = array4;
		this._OpaqueMesh.triangles = array5;
		this._OpaqueMesh.UploadMeshData(true);
		this._OpaqueMesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10000f);
		this._OpaqueMeshFilter.sharedMesh = this._OpaqueMesh;
		this._TransparentMesh.vertices = array;
		this._TransparentMesh.uv = array2;
		this._TransparentMesh.triangles = array5;
		this._TransparentMesh.UploadMeshData(true);
		this._TransparentMesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10000f);
		this._TransparentMeshFilter.sharedMesh = this._TransparentMesh;
	}

	private void BuildMaterials()
	{
		if (this.VignetteShader == null)
		{
			this.VignetteShader = Shader.Find("Oculus/OVRVignette");
		}
		if (this.VignetteShader == null)
		{
			Debug.LogError("Could not find Vignette Shader! Vignette will not be drawn!");
			return;
		}
		if (this._OpaqueMaterial == null)
		{
			this._OpaqueMaterial = new Material(this.VignetteShader)
			{
				name = "Opaque Vignette Material",
				hideFlags = HideFlags.HideAndDontSave,
				renderQueue = 1000
			};
			this._OpaqueMaterial.SetInt("_BlendSrc", 1);
			this._OpaqueMaterial.SetInt("_BlendDst", 0);
			this._OpaqueMaterial.SetInt("_ZWrite", 1);
		}
		this._OpaqueMeshRenderer.sharedMaterial = this._OpaqueMaterial;
		if (this._TransparentMaterial == null)
		{
			this._TransparentMaterial = new Material(this.VignetteShader)
			{
				name = "Transparent Vignette Material",
				hideFlags = HideFlags.HideAndDontSave,
				renderQueue = 4000
			};
			this._TransparentMaterial.SetInt("_BlendSrc", 5);
			this._TransparentMaterial.SetInt("_BlendDst", 10);
			this._TransparentMaterial.SetInt("_ZWrite", 0);
		}
		if (this.Falloff == OVRVignette.FalloffType.Quadratic)
		{
			this._TransparentMaterial.EnableKeyword(OVRVignette.QUADRATIC_FALLOFF);
		}
		else
		{
			this._TransparentMaterial.DisableKeyword(OVRVignette.QUADRATIC_FALLOFF);
		}
		this._TransparentMeshRenderer.sharedMaterial = this._TransparentMaterial;
	}

	private void OnEnable()
	{
		RenderPipelineManager.beginCameraRendering += this.OnBeginCameraRendering;
	}

	private void OnDisable()
	{
		RenderPipelineManager.beginCameraRendering -= this.OnBeginCameraRendering;
		this.DisableRenderers();
	}

	private void Awake()
	{
		this.Initialize();
	}

	private void Initialize()
	{
		if (this._OpaqueMeshRenderer != null && this._TransparentMeshRenderer != null)
		{
			return;
		}
		this._Camera = base.GetComponent<Camera>();
		this._ShaderScaleAndOffset0Property = Shader.PropertyToID("_ScaleAndOffset0");
		this._ShaderScaleAndOffset1Property = Shader.PropertyToID("_ScaleAndOffset1");
		this._ShaderStencilRefProperty = Shader.PropertyToID("_StencilRef");
		this._ShaderStencilOpProperty = Shader.PropertyToID("_StencilOp");
		this._ShaderColorMaskProperty = Shader.PropertyToID("_ColorMask");
		GameObject gameObject = new GameObject("Opaque Vignette")
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		gameObject.transform.SetParent(this._Camera.transform, false);
		this._OpaqueMeshFilter = gameObject.AddComponent<MeshFilter>();
		this._OpaqueMeshRenderer = gameObject.AddComponent<MeshRenderer>();
		this._OpaqueMeshRenderer.receiveShadows = false;
		this._OpaqueMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		this._OpaqueMeshRenderer.lightProbeUsage = LightProbeUsage.Off;
		this._OpaqueMeshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		this._OpaqueMeshRenderer.allowOcclusionWhenDynamic = false;
		this._OpaqueMeshRenderer.enabled = false;
		GameObject gameObject2 = new GameObject("Transparent Vignette")
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		gameObject2.transform.SetParent(this._Camera.transform, false);
		this._TransparentMeshFilter = gameObject2.AddComponent<MeshFilter>();
		this._TransparentMeshRenderer = gameObject2.AddComponent<MeshRenderer>();
		this._TransparentMeshRenderer.receiveShadows = false;
		this._TransparentMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		this._TransparentMeshRenderer.lightProbeUsage = LightProbeUsage.Off;
		this._TransparentMeshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		this._TransparentMeshRenderer.allowOcclusionWhenDynamic = false;
		this._TransparentMeshRenderer.enabled = false;
		this.BuildMeshes();
		this.BuildMaterials();
	}

	private void GetTanFovAndOffsetForStereoEye(Camera.StereoscopicEye eye, out float tanFovX, out float tanFovY, out float offsetX, out float offsetY)
	{
		Matrix4x4 transpose = this._Camera.GetStereoProjectionMatrix(eye).transpose;
		Vector4 vector = transpose * new Vector4(-1f, 0f, 0f, 1f);
		Vector4 vector2 = transpose * new Vector4(1f, 0f, 0f, 1f);
		Vector4 vector3 = transpose * new Vector4(0f, -1f, 0f, 1f);
		Vector4 vector4 = transpose * new Vector4(0f, 1f, 0f, 1f);
		float num = vector.z / vector.x;
		float num2 = vector2.z / vector2.x;
		float num3 = vector3.z / vector3.y;
		float num4 = vector4.z / vector4.y;
		offsetX = -(num + num2) / 2f;
		offsetY = -(num3 + num4) / 2f;
		tanFovX = (num - num2) / 2f;
		tanFovY = (num3 - num4) / 2f;
	}

	private void GetTanFovAndOffsetForMonoEye(out float tanFovX, out float tanFovY, out float offsetX, out float offsetY)
	{
		tanFovY = Mathf.Tan(0.017453292f * this._Camera.fieldOfView * 0.5f);
		tanFovX = tanFovY * this._Camera.aspect;
		offsetX = 0f;
		offsetY = 0f;
	}

	private bool VisibilityTest(float scaleX, float scaleY, float offsetX, float offsetY)
	{
		return new Vector2((1f + Mathf.Abs(offsetX)) / scaleX, (1f + Mathf.Abs(offsetY)) / scaleY).sqrMagnitude > 1f;
	}

	private void Update()
	{
		if (this._OpaqueMaterial == null)
		{
			return;
		}
		float num = Mathf.Tan(this.VignetteFieldOfView * 0.017453292f * 0.5f);
		float num2 = num * this.VignetteAspectRatio;
		float num3 = Mathf.Tan((this.VignetteFieldOfView + this.VignetteFalloffDegrees) * 0.017453292f * 0.5f);
		float num4 = num3 * this.VignetteAspectRatio;
		this._TransparentVignetteVisible = false;
		this._OpaqueVignetteVisible = false;
		for (int i = 0; i < 2; i++)
		{
			float num5;
			float num6;
			float num7;
			float num8;
			if (this._Camera.stereoEnabled)
			{
				this.GetTanFovAndOffsetForStereoEye((Camera.StereoscopicEye)i, out num5, out num6, out num7, out num8);
			}
			else
			{
				this.GetTanFovAndOffsetForMonoEye(out num5, out num6, out num7, out num8);
			}
			float num9 = new Vector2((1f + Mathf.Abs(num7)) / this.VignetteAspectRatio, 1f + Mathf.Abs(num8)).magnitude * 1.01f;
			float num10 = num2 / num5;
			float num11 = num / num6;
			float num12 = num3 / num5;
			float num13 = num4 / num6;
			float x = num9 * this.VignetteAspectRatio;
			float y = num9;
			this._TransparentVignetteVisible |= this.VisibilityTest(num10, num11, num7, num8);
			this._OpaqueVignetteVisible |= this.VisibilityTest(num12, num13, num7, num8);
			this._OpaqueScaleAndOffset0[i] = new Vector4(x, y, num7, num8);
			this._OpaqueScaleAndOffset1[i] = new Vector4(num12, num13, num7, num8);
			this._TransparentScaleAndOffset0[i] = new Vector4(num12, num13, num7, num8);
			this._TransparentScaleAndOffset1[i] = new Vector4(num10, num11, num7, num8);
		}
		this._TransparentVignetteVisible &= (this.VignetteFalloffDegrees > 0f);
		this._OpaqueMaterial.SetVectorArray(this._ShaderScaleAndOffset0Property, this._OpaqueScaleAndOffset0);
		this._OpaqueMaterial.SetVectorArray(this._ShaderScaleAndOffset1Property, this._OpaqueScaleAndOffset1);
		this._OpaqueMaterial.SetInt(this._ShaderStencilOpProperty, this.WriteStencil ? 2 : 0);
		this._OpaqueMaterial.SetInt(this._ShaderStencilRefProperty, this.OpaqueStencilValue);
		this._OpaqueMaterial.SetInt(this._ShaderColorMaskProperty, this.WriteColor ? 15 : 0);
		this._OpaqueMaterial.color = this.VignetteColor;
		this._TransparentMaterial.SetVectorArray(this._ShaderScaleAndOffset0Property, this._TransparentScaleAndOffset0);
		this._TransparentMaterial.SetVectorArray(this._ShaderScaleAndOffset1Property, this._TransparentScaleAndOffset1);
		this._TransparentMaterial.SetInt(this._ShaderStencilOpProperty, this.WriteStencil ? 2 : 0);
		this._TransparentMaterial.SetInt(this._ShaderStencilRefProperty, this.TransparentStencilValue);
		this._TransparentMaterial.SetInt(this._ShaderColorMaskProperty, this.WriteColor ? 15 : 0);
		this._TransparentMaterial.renderQueue = (this.WriteColor ? 4000 : 1000);
		this._TransparentMaterial.color = this.VignetteColor;
	}

	private void EnableRenderers()
	{
		this.Initialize();
		this._OpaqueMeshRenderer.enabled = this._OpaqueVignetteVisible;
		this._TransparentMeshRenderer.enabled = this._TransparentVignetteVisible;
	}

	private void DisableRenderers()
	{
		if (this._OpaqueMeshRenderer != null)
		{
			this._OpaqueMeshRenderer.enabled = false;
		}
		if (this._TransparentMeshRenderer != null)
		{
			this._TransparentMeshRenderer.enabled = false;
		}
	}

	private void OnPreCull()
	{
		this.EnableRenderers();
	}

	private void OnPostRender()
	{
		this.DisableRenderers();
	}

	private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
	{
		if (camera == this._Camera)
		{
			this.EnableRenderers();
			return;
		}
		this.DisableRenderers();
	}

	private static readonly string QUADRATIC_FALLOFF = "QUADRATIC_FALLOFF";

	[SerializeField]
	[HideInInspector]
	private Shader VignetteShader;

	[SerializeField]
	[Tooltip("Controls the number of triangles used for the vignette mesh. Normal is best for most purposes.")]
	private OVRVignette.MeshComplexityLevel MeshComplexity = OVRVignette.MeshComplexityLevel.Normal;

	[SerializeField]
	[Tooltip("Controls how the falloff looks.")]
	private OVRVignette.FalloffType Falloff;

	[Tooltip("The Vertical FOV of the vignette")]
	public float VignetteFieldOfView = 60f;

	[Tooltip("The Aspect ratio of the vignette controls the Horizontal FOV. (Larger numbers are wider)")]
	public float VignetteAspectRatio = 1f;

	[Tooltip("The width of the falloff for the vignette in degrees")]
	public float VignetteFalloffDegrees = 10f;

	[ColorUsage(false)]
	[Tooltip("The color of the vignette. Alpha value is ignored")]
	public Color VignetteColor;

	[Tooltip("Whether the Vignette Should write to the Stencil Buffer.")]
	public bool WriteStencil;

	[Tooltip("If WriteStencil is enabled, the stencil value for the opaque portion of the vignette")]
	public int OpaqueStencilValue;

	[Tooltip("If WriteStencil is enabled, the stencil value for the transparent portion of the vignette")]
	public int TransparentStencilValue;

	[Tooltip("If the Vignette should write color, or only depth/stencil.")]
	public bool WriteColor = true;

	private Camera _Camera;

	private MeshFilter _OpaqueMeshFilter;

	private MeshFilter _TransparentMeshFilter;

	private MeshRenderer _OpaqueMeshRenderer;

	private MeshRenderer _TransparentMeshRenderer;

	private Mesh _OpaqueMesh;

	private Mesh _TransparentMesh;

	private Material _OpaqueMaterial;

	private Material _TransparentMaterial;

	private int _ShaderScaleAndOffset0Property;

	private int _ShaderScaleAndOffset1Property;

	private int _ShaderStencilRefProperty;

	private int _ShaderStencilOpProperty;

	private int _ShaderColorMaskProperty;

	private Vector4[] _TransparentScaleAndOffset0 = new Vector4[2];

	private Vector4[] _TransparentScaleAndOffset1 = new Vector4[2];

	private Vector4[] _OpaqueScaleAndOffset0 = new Vector4[2];

	private Vector4[] _OpaqueScaleAndOffset1 = new Vector4[2];

	private bool _OpaqueVignetteVisible;

	private bool _TransparentVignetteVisible;

	public enum MeshComplexityLevel
	{
		VerySimple,
		Simple,
		Normal,
		Detailed,
		VeryDetailed
	}

	public enum FalloffType
	{
		Linear,
		Quadratic
	}
}
