using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace Meta.XR.EnvironmentDepth
{
	public class EnvironmentDepthManager : MonoBehaviour
	{
		public List<MeshFilter> MaskMeshFilters { get; set; } = new List<MeshFilter>();

		internal event Action<RenderTexture> onDepthTextureUpdate;

		private static IDepthProvider provider
		{
			get
			{
				IDepthProvider result;
				if ((result = EnvironmentDepthManager._provider) == null)
				{
					result = (EnvironmentDepthManager._provider = EnvironmentDepthManager.CreateProvider());
				}
				return result;
			}
		}

		[NotNull]
		private static IDepthProvider CreateProvider()
		{
			if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null && XRGeneralSettings.Instance.Manager.activeLoader != null)
			{
				XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRDisplaySubsystem>();
			}
			Debug.LogError("EnvironmentDepth is disabled. Please enable XR provider in 'Project Settings / XR Plug-in Management'.");
			return new DepthProviderNotSupported();
		}

		public static bool IsSupported
		{
			get
			{
				return EnvironmentDepthManager.provider.IsSupported;
			}
		}

		public bool IsDepthAvailable { get; private set; }

		public OcclusionShadersMode OcclusionShadersMode
		{
			get
			{
				return this._occlusionShadersMode;
			}
			set
			{
				if (this._occlusionShadersMode == value)
				{
					return;
				}
				this._occlusionShadersMode = value;
				if (this.IsDepthAvailable)
				{
					EnvironmentDepthManager.SetOcclusionShaderKeywords(value);
				}
			}
		}

		public bool RemoveHands
		{
			get
			{
				return this._removeHands;
			}
			set
			{
				if (this._removeHands == value)
				{
					return;
				}
				this._removeHands = value;
				if (base.enabled && EnvironmentDepthManager.IsSupported)
				{
					EnvironmentDepthManager.provider.RemoveHands = value;
				}
			}
		}

		public float MaskBias
		{
			get
			{
				return this._maskBias;
			}
			set
			{
				this._maskBias = value;
				if (this._mask != null)
				{
					this._mask._maskMaterial.SetFloat(EnvironmentDepthManager.MaskBiasID, value);
				}
			}
		}

		private void Awake()
		{
			if (!EnvironmentDepthManager.IsSupported)
			{
				return;
			}
			Shader shader = Shader.Find("Meta/EnvironmentDepth/Preprocessing");
			this._preprocessMaterial = new Material(shader);
		}

		private void OnEnable()
		{
			Application.onBeforeRender += this.OnBeforeRender;
			if (!EnvironmentDepthManager.IsSupported)
			{
				Debug.LogError("Environment Depth is not supported. Please check EnvironmentDepthManager.IsSupported before enabling EnvironmentDepthManager.\nOpen 'Oculus -> Tools -> Project Setup Tool' to see requirements.\n");
				base.enabled = false;
				return;
			}
			this._hasPermission = Permission.HasUserAuthorizedPermission("com.oculus.permission.USE_SCENE");
			if (this._hasPermission)
			{
				EnvironmentDepthManager.provider.SetDepthEnabled(true, this._removeHands);
			}
		}

		private void ResetDepthTextureIfAvailable()
		{
			if (this.IsDepthAvailable)
			{
				this.IsDepthAvailable = false;
				Shader.SetGlobalTexture(EnvironmentDepthManager.DepthTextureID, null);
				if (this._occlusionShadersMode != OcclusionShadersMode.None)
				{
					EnvironmentDepthManager.SetOcclusionShaderKeywords(OcclusionShadersMode.None);
				}
			}
		}

		private void OnDisable()
		{
			Application.onBeforeRender -= this.OnBeforeRender;
			this.ResetDepthTextureIfAvailable();
			if (EnvironmentDepthManager.IsSupported && this._hasPermission)
			{
				EnvironmentDepthManager.provider.SetDepthEnabled(false, false);
			}
		}

		private void OnDestroy()
		{
			if (this._preprocessMaterial != null)
			{
				Object.Destroy(this._preprocessMaterial);
			}
			if (this._preprocessTexture != null)
			{
				Object.Destroy(this._preprocessTexture);
			}
			EnvironmentDepthManager.Mask mask = this._mask;
			if (mask == null)
			{
				return;
			}
			mask.Dispose();
		}

		private void OnBeforeRender()
		{
			if (!this._hasPermission)
			{
				if (!Permission.HasUserAuthorizedPermission("com.oculus.permission.USE_SCENE"))
				{
					return;
				}
				this._hasPermission = true;
				EnvironmentDepthManager.provider.SetDepthEnabled(true, this._removeHands);
			}
			Matrix4x4 trackingSpaceWorldToLocalMatrix = this.GetTrackingSpaceWorldToLocalMatrix();
			this.TryFetchDepthTexture(trackingSpaceWorldToLocalMatrix);
			if (!this.IsDepthAvailable)
			{
				return;
			}
			DepthFrameDesc depthFrameDesc = this.frameDescriptors[0];
			Vector4 value = EnvironmentDepthUtils.ComputeNdcToLinearDepthParameters(depthFrameDesc.nearZ, depthFrameDesc.farZ);
			Shader.SetGlobalVector(EnvironmentDepthManager.ZBufferParamsID, value);
			for (int i = 0; i < 2; i++)
			{
				this._reprojectionMatrices[i] = EnvironmentDepthUtils.CalculateReprojection(this.frameDescriptors[i]) * trackingSpaceWorldToLocalMatrix;
			}
			Shader.SetGlobalMatrixArray(EnvironmentDepthManager.ReprojectionMatricesID, this._reprojectionMatrices);
		}

		private void CacheCameraRig()
		{
			if (this._cameraRig == null)
			{
				this._cameraRig = Object.FindObjectOfType<OVRCameraRig>();
			}
		}

		private static void SetOcclusionShaderKeywords(OcclusionShadersMode mode)
		{
			switch (mode)
			{
			case OcclusionShadersMode.None:
				Shader.DisableKeyword("HARD_OCCLUSION");
				Shader.DisableKeyword("SOFT_OCCLUSION");
				return;
			case OcclusionShadersMode.HardOcclusion:
				Shader.DisableKeyword("SOFT_OCCLUSION");
				Shader.EnableKeyword("HARD_OCCLUSION");
				return;
			case OcclusionShadersMode.SoftOcclusion:
				Shader.DisableKeyword("HARD_OCCLUSION");
				Shader.EnableKeyword("SOFT_OCCLUSION");
				return;
			default:
				Debug.LogError(string.Format("Environment Depth: unknown {0} {1}", "OcclusionShadersMode", mode));
				return;
			}
		}

		private void TryFetchDepthTexture(Matrix4x4 trackingSpaceWorldToLocal)
		{
			RenderTexture renderTexture;
			if (!EnvironmentDepthManager.provider.TryGetUpdatedDepthTexture(out renderTexture, this.frameDescriptors))
			{
				return;
			}
			if (renderTexture == null)
			{
				this.ResetDepthTextureIfAvailable();
				return;
			}
			Action<RenderTexture> action = this.onDepthTextureUpdate;
			if (action != null)
			{
				action(renderTexture);
			}
			if (this.MaskMeshFilters != null && this.MaskMeshFilters.Count > 0)
			{
				if (this._mask == null)
				{
					this._mask = new EnvironmentDepthManager.Mask(renderTexture.width, renderTexture.height, this._maskBias);
				}
				renderTexture = this._mask.ApplyMask(renderTexture, this.MaskMeshFilters, trackingSpaceWorldToLocal, this.frameDescriptors);
			}
			Shader.SetGlobalTexture(EnvironmentDepthManager.DepthTextureID, renderTexture);
			if (!this.IsDepthAvailable)
			{
				this.IsDepthAvailable = true;
				if (this._occlusionShadersMode != OcclusionShadersMode.None)
				{
					EnvironmentDepthManager.SetOcclusionShaderKeywords(this._occlusionShadersMode);
				}
			}
			if (this._occlusionShadersMode == OcclusionShadersMode.SoftOcclusion)
			{
				this.PreprocessDepthTexture(renderTexture);
			}
		}

		internal Matrix4x4 GetTrackingSpaceWorldToLocalMatrix()
		{
			if (this.CustomTrackingSpace != null)
			{
				return this.CustomTrackingSpace.worldToLocalMatrix;
			}
			if (!this._isCameraRigCached)
			{
				this._isCameraRigCached = true;
				this.CacheCameraRig();
			}
			if (!(this._cameraRig != null))
			{
				return Matrix4x4.identity;
			}
			return this._cameraRig.trackingSpace.worldToLocalMatrix;
		}

		private void PreprocessDepthTexture(RenderTexture depthTexture)
		{
			if (this._preprocessTexture == null)
			{
				this._preprocessTexture = new RenderTexture(depthTexture.width, depthTexture.height, GraphicsFormat.R16G16B16A16_SFloat, GraphicsFormat.None)
				{
					dimension = TextureDimension.Tex2DArray,
					volumeDepth = 2,
					name = "_preprocessTexture",
					depth = 0
				};
				this._preprocessTexture.Create();
				Shader.SetGlobalTexture(EnvironmentDepthManager.PreprocessedEnvironmentDepthTexture, this._preprocessTexture);
				this._preprocessRenderTargetSetup = new RenderTargetSetup
				{
					color = new RenderBuffer[]
					{
						this._preprocessTexture.colorBuffer
					},
					depth = this._preprocessTexture.depthBuffer,
					depthSlice = -1,
					colorLoad = new RenderBufferLoadAction[]
					{
						RenderBufferLoadAction.DontCare
					},
					colorStore = new RenderBufferStoreAction[1],
					depthLoad = RenderBufferLoadAction.DontCare,
					depthStore = RenderBufferStoreAction.DontCare,
					mipLevel = 0,
					cubemapFace = CubemapFace.Unknown
				};
			}
			Graphics.SetRenderTarget(this._preprocessRenderTargetSetup);
			this._preprocessMaterial.SetPass(0);
			Graphics.DrawProceduralNow(MeshTopology.Triangles, 3, 2);
		}

		[Conditional("UNITY_ASSERTIONS")]
		private static void Log(LogType type, string msg)
		{
			Debug.unityLogger.Log(type, msg);
		}

		public const string HardOcclusionKeyword = "HARD_OCCLUSION";

		public const string SoftOcclusionKeyword = "SOFT_OCCLUSION";

		private const int numViews = 2;

		private static readonly int DepthTextureID = Shader.PropertyToID("_EnvironmentDepthTexture");

		private static readonly int ReprojectionMatricesID = Shader.PropertyToID("_EnvironmentDepthReprojectionMatrices");

		private static readonly int ZBufferParamsID = Shader.PropertyToID("_EnvironmentDepthZBufferParams");

		private static readonly int PreprocessedEnvironmentDepthTexture = Shader.PropertyToID("_PreprocessedEnvironmentDepthTexture");

		private static readonly int MvpMatricesID = Shader.PropertyToID("_DepthMask_MVP_Matrices");

		private static readonly int MaskTextureID = Shader.PropertyToID("_MaskTexture");

		private static readonly int MaskBiasID = Shader.PropertyToID("_MaskBias");

		[SerializeField]
		private OcclusionShadersMode _occlusionShadersMode = OcclusionShadersMode.SoftOcclusion;

		[SerializeField]
		[Tooltip("If set to true, hands will be removed from the depth texture.")]
		private bool _removeHands;

		[SerializeField]
		public Transform CustomTrackingSpace;

		private bool _isCameraRigCached;

		[SerializeField]
		[HideInInspector]
		private OVRCameraRig _cameraRig;

		private static IDepthProvider _provider;

		private bool _hasPermission;

		private Material _preprocessMaterial;

		[CanBeNull]
		private RenderTexture _preprocessTexture;

		private RenderTargetSetup _preprocessRenderTargetSetup;

		internal readonly DepthFrameDesc[] frameDescriptors = new DepthFrameDesc[2];

		private float _maskBias = 0.1f;

		private EnvironmentDepthManager.Mask _mask;

		private readonly Matrix4x4[] _reprojectionMatrices = new Matrix4x4[2];

		private class Mask
		{
			internal Mask(int width, int height, float bias)
			{
				Shader shader = Shader.Find("Meta/EnvironmentDepth/DepthMask");
				this._maskMaterial = new Material(shader)
				{
					enableInstancing = true
				};
				this._maskMaterial.SetFloat(EnvironmentDepthManager.MaskBiasID, bias);
				this._maskDepthRt = new RenderTexture(width, height, GraphicsFormat.R16_UNorm, GraphicsFormat.D16_UNorm)
				{
					dimension = TextureDimension.Tex2DArray,
					volumeDepth = 2
				};
				this._maskedDepthTexture = new RenderTexture(width, height, GraphicsFormat.R16_UNorm, GraphicsFormat.None)
				{
					dimension = TextureDimension.Tex2DArray,
					volumeDepth = 2,
					depth = 0
				};
				this._maskCommandBuffer = new CommandBuffer();
			}

			internal RenderTexture ApplyMask(RenderTexture depthTexture, List<MeshFilter> meshFilters, Matrix4x4 trackingSpaceWorldToLocal, DepthFrameDesc[] frameDescriptors)
			{
				Matrix4x4 proj;
				Matrix4x4 rhs;
				EnvironmentDepthUtils.CalculateDepthCameraMatrices(frameDescriptors[0], out proj, out rhs);
				Matrix4x4 proj2;
				Matrix4x4 rhs2;
				EnvironmentDepthUtils.CalculateDepthCameraMatrices(frameDescriptors[1], out proj2, out rhs2);
				this._maskCommandBuffer.SetRenderTarget(new RenderTargetIdentifier(this._maskDepthRt, 0, CubemapFace.Unknown, -1), RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);
				this._maskCommandBuffer.ClearRenderTarget(true, true, Color.white);
				foreach (MeshFilter meshFilter in meshFilters)
				{
					if (meshFilter == null || meshFilter.sharedMesh == null)
					{
						Debug.LogError("MeshFilter or sharedMesh is null.");
					}
					else
					{
						this._mvpMatrices[0] = GL.GetGPUProjectionMatrix(proj, true) * rhs * trackingSpaceWorldToLocal * meshFilter.transform.localToWorldMatrix;
						this._mvpMatrices[1] = GL.GetGPUProjectionMatrix(proj2, true) * rhs2 * trackingSpaceWorldToLocal * meshFilter.transform.localToWorldMatrix;
						this._maskCommandBuffer.SetGlobalMatrixArray(EnvironmentDepthManager.MvpMatricesID, this._mvpMatrices);
						this._maskCommandBuffer.DrawMeshInstancedProcedural(meshFilter.sharedMesh, 0, this._maskMaterial, 0, 2, null);
					}
				}
				this._maskMaterial.SetTexture(EnvironmentDepthManager.DepthTextureID, depthTexture);
				this._maskMaterial.SetTexture(EnvironmentDepthManager.MaskTextureID, this._maskDepthRt);
				this._maskCommandBuffer.SetRenderTarget(new RenderTargetIdentifier(this._maskedDepthTexture, 0, CubemapFace.Unknown, -1), RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);
				this._maskCommandBuffer.DrawProcedural(Matrix4x4.identity, this._maskMaterial, 1, MeshTopology.Triangles, 3, 2);
				Graphics.ExecuteCommandBuffer(this._maskCommandBuffer);
				this._maskCommandBuffer.Clear();
				return this._maskedDepthTexture;
			}

			internal void Dispose()
			{
				Object.Destroy(this._maskMaterial);
				Object.Destroy(this._maskDepthRt);
				Object.Destroy(this._maskedDepthTexture);
				this._maskCommandBuffer.Dispose();
			}

			internal readonly Material _maskMaterial;

			private readonly RenderTexture _maskDepthRt;

			private readonly RenderTexture _maskedDepthTexture;

			private readonly CommandBuffer _maskCommandBuffer;

			private readonly Matrix4x4[] _mvpMatrices = new Matrix4x4[2];
		}
	}
}
