using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering
{
	public class XRPass
	{
		public XRPass()
		{
			this.m_Views = new List<XRView>(2);
			this.m_OcclusionMesh = new XROcclusionMesh(this);
			this.m_VisibleMesh = new XRVisibleMesh(this);
		}

		public static XRPass CreateDefault(XRPassCreateInfo createInfo)
		{
			XRPass xrpass = GenericPool<XRPass>.Get();
			xrpass.InitBase(createInfo);
			return xrpass;
		}

		public virtual void Release()
		{
			this.m_VisibleMesh.Dispose();
			GenericPool<XRPass>.Release(this);
		}

		public bool enabled
		{
			get
			{
				return this.viewCount > 0;
			}
		}

		public bool supportsFoveatedRendering
		{
			get
			{
				return this.enabled && this.foveatedRenderingInfo != IntPtr.Zero && XRSystem.foveatedRenderingCaps > FoveatedRenderingCaps.None;
			}
		}

		public bool copyDepth { get; private set; }

		public bool hasMotionVectorPass { get; private set; }

		public bool spaceWarpRightHandedNDC { get; private set; }

		public bool isFirstCameraPass
		{
			get
			{
				return this.multipassId == 0;
			}
		}

		public bool isLastCameraPass
		{
			get
			{
				return (this.multipassId == 1 && this.viewCount <= 1) || (this.multipassId == 0 && this.viewCount > 1) || (this.multipassId == 0 && this.viewCount == 0);
			}
		}

		public int multipassId { get; private set; }

		public int cullingPassId { get; private set; }

		public int renderTargetScaledWidth { get; private set; }

		public int renderTargetScaledHeight { get; private set; }

		public RenderTargetIdentifier renderTarget { get; private set; }

		public RenderTextureDescriptor renderTargetDesc { get; private set; }

		public RenderTargetIdentifier motionVectorRenderTarget { get; private set; }

		public RenderTextureDescriptor motionVectorRenderTargetDesc { get; private set; }

		public ScriptableCullingParameters cullingParams { get; private set; }

		public int viewCount
		{
			get
			{
				return this.m_Views.Count;
			}
		}

		public bool singlePassEnabled
		{
			get
			{
				return this.viewCount > 1;
			}
		}

		public IntPtr foveatedRenderingInfo { get; private set; }

		public bool isHDRDisplayOutputActive
		{
			get
			{
				HDROutputSettings hdrOutputSettings = XRSystem.GetActiveDisplay().hdrOutputSettings;
				return hdrOutputSettings != null && hdrOutputSettings.active;
			}
		}

		public ColorGamut hdrDisplayOutputColorGamut
		{
			get
			{
				HDROutputSettings hdrOutputSettings = XRSystem.GetActiveDisplay().hdrOutputSettings;
				if (hdrOutputSettings == null)
				{
					return ColorGamut.sRGB;
				}
				return hdrOutputSettings.displayColorGamut;
			}
		}

		public HDROutputUtils.HDRDisplayInformation hdrDisplayOutputInformation
		{
			get
			{
				HDROutputSettings hdrOutputSettings = XRSystem.GetActiveDisplay().hdrOutputSettings;
				int maxFullFrameToneMapLuminance = (hdrOutputSettings != null) ? hdrOutputSettings.maxFullFrameToneMapLuminance : -1;
				HDROutputSettings hdrOutputSettings2 = XRSystem.GetActiveDisplay().hdrOutputSettings;
				int maxToneMapLuminance = (hdrOutputSettings2 != null) ? hdrOutputSettings2.maxToneMapLuminance : -1;
				HDROutputSettings hdrOutputSettings3 = XRSystem.GetActiveDisplay().hdrOutputSettings;
				int minToneMapLuminance = (hdrOutputSettings3 != null) ? hdrOutputSettings3.minToneMapLuminance : -1;
				HDROutputSettings hdrOutputSettings4 = XRSystem.GetActiveDisplay().hdrOutputSettings;
				return new HDROutputUtils.HDRDisplayInformation(maxFullFrameToneMapLuminance, maxToneMapLuminance, minToneMapLuminance, (hdrOutputSettings4 != null) ? hdrOutputSettings4.paperWhiteNits : 160f);
			}
		}

		public float occlusionMeshScale { get; private set; }

		public Matrix4x4 GetProjMatrix(int viewIndex = 0)
		{
			return this.m_Views[viewIndex].projMatrix;
		}

		public Matrix4x4 GetViewMatrix(int viewIndex = 0)
		{
			return this.m_Views[viewIndex].viewMatrix;
		}

		public bool GetPrevViewValid(int viewIndex = 0)
		{
			return this.m_Views[viewIndex].isPrevViewMatrixValid;
		}

		public Matrix4x4 GetPrevViewMatrix(int viewIndex = 0)
		{
			return this.m_Views[viewIndex].prevViewMatrix;
		}

		public Rect GetViewport(int viewIndex = 0)
		{
			return this.m_Views[viewIndex].viewport;
		}

		public Mesh GetOcclusionMesh(int viewIndex = 0)
		{
			return this.m_Views[viewIndex].occlusionMesh;
		}

		public Mesh GetVisibleMesh(int viewIndex = 0)
		{
			return this.m_Views[viewIndex].visibleMesh;
		}

		public int GetTextureArraySlice(int viewIndex = 0)
		{
			return this.m_Views[viewIndex].textureArraySlice;
		}

		public void StartSinglePass(CommandBuffer cmd)
		{
			if (!this.enabled || !this.singlePassEnabled)
			{
				return;
			}
			if (this.viewCount > TextureXR.slices)
			{
				throw new NotImplementedException(string.Format("Invalid XR setup for single-pass, trying to render too many views! Max supported: {0}", TextureXR.slices));
			}
			if (SystemInfo.supportsMultiview)
			{
				cmd.EnableKeyword(SinglepassKeywords.STEREO_MULTIVIEW_ON);
				return;
			}
			cmd.EnableKeyword(SinglepassKeywords.STEREO_INSTANCING_ON);
			cmd.SetInstanceMultiplier((uint)this.viewCount);
		}

		public void StartSinglePass(IRasterCommandBuffer cmd)
		{
			this.StartSinglePass((cmd as BaseCommandBuffer).m_WrappedCommandBuffer);
		}

		public void StopSinglePass(CommandBuffer cmd)
		{
			if (this.enabled && this.singlePassEnabled)
			{
				if (SystemInfo.supportsMultiview)
				{
					cmd.DisableKeyword(SinglepassKeywords.STEREO_MULTIVIEW_ON);
					return;
				}
				cmd.DisableKeyword(SinglepassKeywords.STEREO_INSTANCING_ON);
				cmd.SetInstanceMultiplier(1U);
			}
		}

		public void StopSinglePass(BaseCommandBuffer cmd)
		{
			this.StopSinglePass(cmd.m_WrappedCommandBuffer);
		}

		public bool hasValidOcclusionMesh
		{
			get
			{
				return this.m_OcclusionMesh.hasValidOcclusionMesh;
			}
		}

		public bool hasValidVisibleMesh
		{
			get
			{
				return this.m_VisibleMesh.hasValidVisibleMesh && XRSystem.GetUseVisibilityMesh();
			}
		}

		public void RenderOcclusionMesh(CommandBuffer cmd, bool renderIntoTexture = false)
		{
			if (this.occlusionMeshScale > 0f)
			{
				this.m_OcclusionMesh.RenderOcclusionMesh(cmd, this.occlusionMeshScale, renderIntoTexture);
			}
		}

		public void RenderOcclusionMesh(RasterCommandBuffer cmd, bool renderIntoTexture = false)
		{
			if (this.occlusionMeshScale > 0f)
			{
				this.m_OcclusionMesh.RenderOcclusionMesh(cmd.m_WrappedCommandBuffer, this.occlusionMeshScale, renderIntoTexture);
			}
		}

		public void RenderVisibleMeshCustomMaterial(RasterCommandBuffer cmd, float occlusionMeshScale, Material material, MaterialPropertyBlock materialBlock, int shaderPass, bool renderIntoTexture = false)
		{
			if (occlusionMeshScale > 0f)
			{
				this.m_VisibleMesh.RenderVisibleMeshCustomMaterial(cmd.m_WrappedCommandBuffer, occlusionMeshScale, material, materialBlock, shaderPass, renderIntoTexture);
			}
		}

		public void RenderVisibleMeshCustomMaterial(CommandBuffer cmd, float occlusionMeshScale, Material material, MaterialPropertyBlock materialBlock, int shaderPass = 0, bool renderIntoTexture = false)
		{
			if (occlusionMeshScale > 0f)
			{
				this.m_VisibleMesh.RenderVisibleMeshCustomMaterial(cmd, occlusionMeshScale, material, materialBlock, shaderPass, renderIntoTexture);
			}
		}

		public void RenderDebugXRViewsFrustum()
		{
			for (int i = 0; i < this.m_Views.Count; i++)
			{
				XRView xrview = this.m_Views[i];
				Vector3[] array = CoreUtils.CalculateViewSpaceCorners(xrview.projMatrix, 10f);
				Vector3 start = -xrview.viewMatrix.GetColumn(3);
				for (int j = 0; j < 4; j++)
				{
					Debug.DrawLine(start, xrview.viewMatrix.MultiplyPoint(array[j]), (i == 0) ? Color.green : Color.red);
				}
			}
		}

		public Vector4 ApplyXRViewCenterOffset(Vector2 center)
		{
			Vector4 zero = Vector4.zero;
			float num = 0.5f - center.x;
			float num2 = 0.5f - center.y;
			zero.x = this.m_Views[0].eyeCenterUV.x - num;
			zero.y = this.m_Views[0].eyeCenterUV.y - num2;
			if (this.singlePassEnabled)
			{
				zero.z = this.m_Views[1].eyeCenterUV.x - num;
				zero.w = this.m_Views[1].eyeCenterUV.y - num2;
			}
			return zero;
		}

		internal void AssignView(int viewId, XRView xrView)
		{
			if (viewId < 0 || viewId >= this.m_Views.Count)
			{
				throw new ArgumentOutOfRangeException("viewId");
			}
			this.m_Views[viewId] = xrView;
		}

		internal void AssignCullingParams(int cullingPassId, ScriptableCullingParameters cullingParams)
		{
			cullingParams.cullingOptions &= ~CullingOptions.Stereo;
			this.cullingPassId = cullingPassId;
			this.cullingParams = cullingParams;
		}

		internal void UpdateCombinedOcclusionMesh()
		{
			this.m_OcclusionMesh.UpdateCombinedMesh();
			this.m_VisibleMesh.UpdateCombinedMesh();
		}

		public void InitBase(XRPassCreateInfo createInfo)
		{
			this.m_Views.Clear();
			this.copyDepth = createInfo.copyDepth;
			this.multipassId = createInfo.multipassId;
			this.AssignCullingParams(createInfo.cullingPassId, createInfo.cullingParameters);
			this.renderTarget = new RenderTargetIdentifier(createInfo.renderTarget, 0, CubemapFace.Unknown, -1);
			this.renderTargetDesc = createInfo.renderTargetDesc;
			this.renderTargetScaledWidth = createInfo.renderTargetScaledWidth;
			this.renderTargetScaledHeight = createInfo.renderTargetScaledHeight;
			this.motionVectorRenderTarget = new RenderTargetIdentifier(createInfo.motionVectorRenderTarget, 0, CubemapFace.Unknown, -1);
			this.motionVectorRenderTargetDesc = createInfo.motionVectorRenderTargetDesc;
			this.hasMotionVectorPass = createInfo.hasMotionVectorPass;
			this.spaceWarpRightHandedNDC = createInfo.spaceWarpRightHandedNDC;
			this.m_OcclusionMesh.SetMaterial(createInfo.occlusionMeshMaterial);
			this.occlusionMeshScale = createInfo.occlusionMeshScale;
			this.foveatedRenderingInfo = createInfo.foveatedRenderingInfo;
		}

		internal void AddView(XRView xrView)
		{
			if (this.m_Views.Count < TextureXR.slices)
			{
				this.m_Views.Add(xrView);
				return;
			}
			throw new NotImplementedException(string.Format("Invalid XR setup for single-pass, trying to add too many views! Max supported: {0}", TextureXR.slices));
		}

		private readonly List<XRView> m_Views;

		private readonly XROcclusionMesh m_OcclusionMesh;

		private readonly XRVisibleMesh m_VisibleMesh;
	}
}
