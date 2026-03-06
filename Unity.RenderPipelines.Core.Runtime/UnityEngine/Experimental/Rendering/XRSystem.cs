using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace UnityEngine.Experimental.Rendering
{
	public static class XRSystem
	{
		public static XRDisplaySubsystem GetActiveDisplay()
		{
			return XRSystem.s_Display;
		}

		public static bool displayActive
		{
			get
			{
				return XRSystem.s_Display != null && XRSystem.s_Display.running;
			}
		}

		public static bool isHDRDisplayOutputActive
		{
			get
			{
				XRDisplaySubsystem xrdisplaySubsystem = XRSystem.s_Display;
				bool? flag;
				if (xrdisplaySubsystem == null)
				{
					flag = null;
				}
				else
				{
					HDROutputSettings hdrOutputSettings = xrdisplaySubsystem.hdrOutputSettings;
					flag = ((hdrOutputSettings != null) ? new bool?(hdrOutputSettings.active) : null);
				}
				bool? flag2 = flag;
				return flag2.GetValueOrDefault();
			}
		}

		public static bool singlePassAllowed { get; set; } = true;

		public static FoveatedRenderingCaps foveatedRenderingCaps { get; set; }

		public static bool dumpDebugInfo { get; set; } = false;

		public static void Initialize(Func<XRPassCreateInfo, XRPass> passAllocator, Shader occlusionMeshPS, Shader mirrorViewPS)
		{
			if (passAllocator == null)
			{
				throw new ArgumentNullException("passCreator");
			}
			XRSystem.s_PassAllocator = passAllocator;
			XRSystem.RefreshDeviceInfo();
			XRSystem.foveatedRenderingCaps = SystemInfo.foveatedRenderingCaps;
			if (occlusionMeshPS != null && XRSystem.s_OcclusionMeshMaterial == null)
			{
				XRSystem.s_OcclusionMeshMaterial = CoreUtils.CreateEngineMaterial(occlusionMeshPS);
			}
			if (mirrorViewPS != null && XRSystem.s_MirrorViewMaterial == null)
			{
				XRSystem.s_MirrorViewMaterial = CoreUtils.CreateEngineMaterial(mirrorViewPS);
			}
			if (XRGraphicsAutomatedTests.enabled)
			{
				XRSystem.SetLayoutOverride(new Action<XRLayout, Camera>(XRGraphicsAutomatedTests.OverrideLayout));
			}
			SinglepassKeywords.STEREO_MULTIVIEW_ON = GlobalKeyword.Create("STEREO_MULTIVIEW_ON");
			SinglepassKeywords.STEREO_INSTANCING_ON = GlobalKeyword.Create("STEREO_INSTANCING_ON");
		}

		public static void SetDisplayMSAASamples(MSAASamples msaaSamples)
		{
			if (XRSystem.s_MSAASamples == msaaSamples)
			{
				return;
			}
			XRSystem.s_MSAASamples = msaaSamples;
			SubsystemManager.GetSubsystems<XRDisplaySubsystem>(XRSystem.s_DisplayList);
			foreach (XRDisplaySubsystem xrdisplaySubsystem in XRSystem.s_DisplayList)
			{
				xrdisplaySubsystem.SetMSAALevel((int)XRSystem.s_MSAASamples);
			}
		}

		public static MSAASamples GetDisplayMSAASamples()
		{
			return XRSystem.s_MSAASamples;
		}

		internal static void SetOcclusionMeshScale(float occlusionMeshScale)
		{
			XRSystem.s_OcclusionMeshScaling = occlusionMeshScale;
		}

		internal static float GetOcclusionMeshScale()
		{
			return XRSystem.s_OcclusionMeshScaling;
		}

		internal static void SetUseVisibilityMesh(bool useVisibilityMesh)
		{
			XRSystem.s_UseVisibilityMesh = useVisibilityMesh;
		}

		internal static bool GetUseVisibilityMesh()
		{
			return XRSystem.s_UseVisibilityMesh;
		}

		internal static void SetMirrorViewMode(int mirrorBlitMode)
		{
			if (XRSystem.s_Display == null)
			{
				return;
			}
			XRSystem.s_Display.SetPreferredMirrorBlitMode(mirrorBlitMode);
		}

		internal static int GetMirrorViewMode()
		{
			if (XRSystem.s_Display == null)
			{
				return -6;
			}
			return XRSystem.s_Display.GetPreferredMirrorBlitMode();
		}

		public static void SetRenderScale(float renderScale)
		{
			SubsystemManager.GetSubsystems<XRDisplaySubsystem>(XRSystem.s_DisplayList);
			foreach (XRDisplaySubsystem xrdisplaySubsystem in XRSystem.s_DisplayList)
			{
				xrdisplaySubsystem.scaleOfAllRenderTargets = renderScale;
			}
		}

		public static float GetRenderViewportScale()
		{
			return XRSystem.s_Display.scaleOfAllViewports;
		}

		public static float GetDynamicResolutionScale()
		{
			return XRSystem.s_Display.globalDynamicScale;
		}

		public static int ScaleTextureWidthForXR(RenderTexture texture)
		{
			return XRSystem.s_Display.ScaledTextureWidth(texture);
		}

		public static int ScaleTextureHeightForXR(RenderTexture texture)
		{
			return XRSystem.s_Display.ScaledTextureHeight(texture);
		}

		public static XRLayout NewLayout()
		{
			XRSystem.RefreshDeviceInfo();
			return XRSystem.s_Layout.New();
		}

		public static void EndLayout()
		{
			if (XRSystem.dumpDebugInfo)
			{
				XRSystem.s_Layout.top.LogDebugInfo();
			}
			XRSystem.s_Layout.Release();
		}

		public static void RenderMirrorView(CommandBuffer cmd, Camera camera)
		{
			XRMirrorView.RenderMirrorView(cmd, camera, XRSystem.s_MirrorViewMaterial, XRSystem.s_Display);
		}

		public static void Dispose()
		{
			if (XRSystem.s_OcclusionMeshMaterial != null)
			{
				CoreUtils.Destroy(XRSystem.s_OcclusionMeshMaterial);
				XRSystem.s_OcclusionMeshMaterial = null;
			}
			if (XRSystem.s_MirrorViewMaterial != null)
			{
				CoreUtils.Destroy(XRSystem.s_MirrorViewMaterial);
				XRSystem.s_MirrorViewMaterial = null;
			}
		}

		internal static void SetDisplayZRange(float zNear, float zFar)
		{
			if (XRSystem.s_Display != null)
			{
				XRSystem.s_Display.zNear = zNear;
				XRSystem.s_Display.zFar = zFar;
			}
		}

		private static void SetLayoutOverride(Action<XRLayout, Camera> action)
		{
			XRSystem.s_LayoutOverride = action;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
		private static void XRSystemInit()
		{
			if (GraphicsSettings.currentRenderPipeline != null)
			{
				XRSystem.RefreshDeviceInfo();
			}
		}

		private static void RefreshDeviceInfo()
		{
			SubsystemManager.GetSubsystems<XRDisplaySubsystem>(XRSystem.s_DisplayList);
			if (XRSystem.s_DisplayList.Count <= 0)
			{
				XRSystem.s_Display = null;
				return;
			}
			if (XRSystem.s_DisplayList.Count > 1)
			{
				throw new NotImplementedException("Only one XR display is supported!");
			}
			XRSystem.s_Display = XRSystem.s_DisplayList[0];
			XRSystem.s_Display.disableLegacyRenderer = true;
			XRSystem.s_Display.sRGB = (QualitySettings.activeColorSpace == ColorSpace.Linear);
			XRSystem.s_Display.textureLayout = XRDisplaySubsystem.TextureLayout.Texture2DArray;
			TextureXR.maxViews = Math.Max(TextureXR.slices, 2);
		}

		internal static void CreateDefaultLayout(Camera camera, XRLayout layout)
		{
			XRSystem.<>c__DisplayClass50_0 CS$<>8__locals1;
			CS$<>8__locals1.camera = camera;
			if (XRSystem.s_Display == null)
			{
				throw new NullReferenceException("s_Display");
			}
			for (int i = 0; i < XRSystem.s_Display.GetRenderPassCount(); i++)
			{
				XRDisplaySubsystem.XRRenderPass xrrenderPass;
				XRSystem.s_Display.GetRenderPass(i, out xrrenderPass);
				ScriptableCullingParameters cullingParameters;
				XRSystem.s_Display.GetCullingParameters(CS$<>8__locals1.camera, xrrenderPass.cullingPassIndex, out cullingParameters);
				int renderParameterCount = xrrenderPass.GetRenderParameterCount();
				if (XRSystem.CanUseSinglePass(CS$<>8__locals1.camera, xrrenderPass))
				{
					XRPassCreateInfo arg = XRSystem.BuildPass(xrrenderPass, cullingParameters, layout);
					XRPass xrPass = XRSystem.s_PassAllocator(arg);
					for (int j = 0; j < renderParameterCount; j++)
					{
						XRSystem.<CreateDefaultLayout>g__AddViewToPass|50_0(xrPass, xrrenderPass, j, ref CS$<>8__locals1);
					}
					layout.AddPass(CS$<>8__locals1.camera, xrPass);
				}
				else
				{
					for (int k = 0; k < renderParameterCount; k++)
					{
						XRPassCreateInfo arg2 = XRSystem.BuildPass(xrrenderPass, cullingParameters, layout);
						XRPass xrPass2 = XRSystem.s_PassAllocator(arg2);
						XRSystem.<CreateDefaultLayout>g__AddViewToPass|50_0(xrPass2, xrrenderPass, k, ref CS$<>8__locals1);
						layout.AddPass(CS$<>8__locals1.camera, xrPass2);
					}
				}
			}
			Action<XRLayout, Camera> action = XRSystem.s_LayoutOverride;
			if (action == null)
			{
				return;
			}
			action(layout, CS$<>8__locals1.camera);
		}

		internal static void ReconfigurePass(XRPass xrPass, Camera camera)
		{
			if (xrPass.enabled && XRSystem.s_Display != null)
			{
				XRDisplaySubsystem.XRRenderPass xrrenderPass;
				XRSystem.s_Display.GetRenderPass(xrPass.multipassId, out xrrenderPass);
				ScriptableCullingParameters cullingParams;
				XRSystem.s_Display.GetCullingParameters(camera, xrrenderPass.cullingPassIndex, out cullingParams);
				xrPass.AssignCullingParams(xrrenderPass.cullingPassIndex, cullingParams);
				for (int i = 0; i < xrrenderPass.GetRenderParameterCount(); i++)
				{
					XRDisplaySubsystem.XRRenderParameter renderParameter;
					xrrenderPass.GetRenderParameter(camera, i, out renderParameter);
					xrPass.AssignView(i, XRSystem.BuildView(xrrenderPass, renderParameter));
				}
				Action<XRLayout, Camera> action = XRSystem.s_LayoutOverride;
				if (action == null)
				{
					return;
				}
				action(XRSystem.s_Layout.top, camera);
			}
		}

		private static bool CanUseSinglePass(Camera camera, XRDisplaySubsystem.XRRenderPass renderPass)
		{
			if (!XRSystem.singlePassAllowed)
			{
				return false;
			}
			if (renderPass.renderTargetDesc.dimension != TextureDimension.Tex2DArray)
			{
				return false;
			}
			if (renderPass.GetRenderParameterCount() != 2 || renderPass.renderTargetDesc.volumeDepth != 2)
			{
				return false;
			}
			XRDisplaySubsystem.XRRenderParameter xrrenderParameter;
			renderPass.GetRenderParameter(camera, 0, out xrrenderParameter);
			XRDisplaySubsystem.XRRenderParameter xrrenderParameter2;
			renderPass.GetRenderParameter(camera, 1, out xrrenderParameter2);
			return xrrenderParameter.textureArraySlice == 0 && xrrenderParameter2.textureArraySlice == 1 && !(xrrenderParameter.viewport != xrrenderParameter2.viewport);
		}

		private static XRView BuildView(XRDisplaySubsystem.XRRenderPass renderPass, XRDisplaySubsystem.XRRenderParameter renderParameter)
		{
			Rect viewport = renderParameter.viewport;
			viewport.x *= (float)renderPass.renderTargetScaledWidth;
			viewport.width *= (float)renderPass.renderTargetScaledWidth;
			viewport.y *= (float)renderPass.renderTargetScaledHeight;
			viewport.height *= (float)renderPass.renderTargetScaledHeight;
			Mesh occlusionMesh = XRGraphicsAutomatedTests.running ? null : renderParameter.occlusionMesh;
			Mesh visibleMesh = XRGraphicsAutomatedTests.running ? null : renderParameter.visibleMesh;
			return new XRView(renderParameter.projection, renderParameter.view, renderParameter.previousView, renderParameter.isPreviousViewValid, viewport, occlusionMesh, visibleMesh, renderParameter.textureArraySlice);
		}

		private static RenderTextureDescriptor XrRenderTextureDescToUnityRenderTextureDesc(RenderTextureDescriptor xrDesc)
		{
			return new RenderTextureDescriptor(xrDesc.width, xrDesc.height, xrDesc.graphicsFormat, xrDesc.depthStencilFormat, xrDesc.mipCount)
			{
				dimension = xrDesc.dimension,
				msaaSamples = xrDesc.msaaSamples,
				volumeDepth = xrDesc.volumeDepth,
				vrUsage = xrDesc.vrUsage,
				sRGB = xrDesc.sRGB,
				shadowSamplingMode = xrDesc.shadowSamplingMode
			};
		}

		private static XRPassCreateInfo BuildPass(XRDisplaySubsystem.XRRenderPass xrRenderPass, ScriptableCullingParameters cullingParameters, XRLayout layout)
		{
			return new XRPassCreateInfo
			{
				renderTarget = xrRenderPass.renderTarget,
				renderTargetDesc = XRSystem.XrRenderTextureDescToUnityRenderTextureDesc(xrRenderPass.renderTargetDesc),
				renderTargetScaledWidth = xrRenderPass.renderTargetScaledWidth,
				renderTargetScaledHeight = xrRenderPass.renderTargetScaledHeight,
				hasMotionVectorPass = xrRenderPass.hasMotionVectorPass,
				motionVectorRenderTarget = xrRenderPass.motionVectorRenderTarget,
				motionVectorRenderTargetDesc = XRSystem.XrRenderTextureDescToUnityRenderTextureDesc(xrRenderPass.motionVectorRenderTargetDesc),
				cullingParameters = cullingParameters,
				occlusionMeshMaterial = XRSystem.s_OcclusionMeshMaterial,
				occlusionMeshScale = XRSystem.GetOcclusionMeshScale(),
				foveatedRenderingInfo = xrRenderPass.foveatedRenderingInfo,
				multipassId = layout.GetActivePasses().Count,
				cullingPassId = xrRenderPass.cullingPassIndex,
				copyDepth = xrRenderPass.shouldFillOutDepth,
				spaceWarpRightHandedNDC = xrRenderPass.spaceWarpRightHandedNDC,
				xrSdkRenderPass = xrRenderPass
			};
		}

		[CompilerGenerated]
		internal static void <CreateDefaultLayout>g__AddViewToPass|50_0(XRPass xrPass, XRDisplaySubsystem.XRRenderPass renderPass, int renderParamIndex, ref XRSystem.<>c__DisplayClass50_0 A_3)
		{
			XRDisplaySubsystem.XRRenderParameter renderParameter;
			renderPass.GetRenderParameter(A_3.camera, renderParamIndex, out renderParameter);
			xrPass.AddView(XRSystem.BuildView(renderPass, renderParameter));
		}

		private static XRLayoutStack s_Layout = new XRLayoutStack();

		private static Func<XRPassCreateInfo, XRPass> s_PassAllocator = null;

		private static List<XRDisplaySubsystem> s_DisplayList = new List<XRDisplaySubsystem>();

		private static XRDisplaySubsystem s_Display;

		private static MSAASamples s_MSAASamples = MSAASamples.None;

		private static float s_OcclusionMeshScaling = 1f;

		private static bool s_UseVisibilityMesh = true;

		private static Material s_OcclusionMeshMaterial;

		private static Material s_MirrorViewMaterial;

		private static Action<XRLayout, Camera> s_LayoutOverride = null;

		public static readonly XRPass emptyPass = new XRPass();
	}
}
