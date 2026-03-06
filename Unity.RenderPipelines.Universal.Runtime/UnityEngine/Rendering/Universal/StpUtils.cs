using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal static class StpUtils
	{
		private static void CalculateJitter(int frameIndex, out Vector2 jitter, out bool allowScaling)
		{
			jitter = -STP.Jit16(frameIndex);
			allowScaling = false;
		}

		private static void PopulateStpConfig(UniversalCameraData cameraData, TextureHandle inputColor, TextureHandle inputDepth, TextureHandle inputMotion, int debugViewIndex, TextureHandle debugView, TextureHandle destination, Texture2D noiseTexture, out STP.Config config)
		{
			UniversalAdditionalCameraData universalAdditionalCameraData;
			cameraData.camera.TryGetComponent<UniversalAdditionalCameraData>(out universalAdditionalCameraData);
			MotionVectorsPersistentData motionVectorsPersistentData = universalAdditionalCameraData.motionVectorsPersistentData;
			config.enableHwDrs = false;
			config.enableTexArray = (cameraData.xr.enabled && cameraData.xr.singlePassEnabled);
			config.enableMotionScaling = true;
			config.noiseTexture = noiseTexture;
			config.inputColor = inputColor;
			config.inputDepth = inputDepth;
			config.inputMotion = inputMotion;
			config.inputStencil = TextureHandle.nullHandle;
			config.stencilMask = 0;
			config.debugView = debugView;
			config.destination = destination;
			StpHistory stpHistory = cameraData.stpHistory;
			int num = (cameraData.xr.enabled && !cameraData.xr.singlePassEnabled) ? cameraData.xr.multipassId : 0;
			config.historyContext = stpHistory.GetHistoryContext(num);
			config.nearPlane = cameraData.camera.nearClipPlane;
			config.farPlane = cameraData.camera.farClipPlane;
			config.frameIndex = TemporalAA.CalculateTaaFrameIndex(ref cameraData.taaSettings);
			config.hasValidHistory = !cameraData.resetHistory;
			config.debugViewIndex = debugViewIndex;
			config.deltaTime = motionVectorsPersistentData.deltaTime;
			config.lastDeltaTime = motionVectorsPersistentData.lastDeltaTime;
			config.currentImageSize = new Vector2Int(cameraData.cameraTargetDescriptor.width, cameraData.cameraTargetDescriptor.height);
			config.priorImageSize = config.currentImageSize;
			config.outputImageSize = new Vector2Int(cameraData.pixelWidth, cameraData.pixelHeight);
			int num2 = cameraData.xr.enabled ? cameraData.xr.viewCount : 1;
			for (int i = 0; i < num2; i++)
			{
				int num3 = i + num;
				STP.PerViewConfig perViewConfig;
				perViewConfig.currentProj = motionVectorsPersistentData.projectionStereo[num3];
				perViewConfig.lastProj = motionVectorsPersistentData.previousProjectionStereo[num3];
				perViewConfig.lastLastProj = motionVectorsPersistentData.previousPreviousProjectionStereo[num3];
				perViewConfig.currentView = motionVectorsPersistentData.viewStereo[num3];
				perViewConfig.lastView = motionVectorsPersistentData.previousViewStereo[num3];
				perViewConfig.lastLastView = motionVectorsPersistentData.previousPreviousViewStereo[num3];
				Vector3 worldSpaceCameraPos = motionVectorsPersistentData.worldSpaceCameraPos;
				Vector3 previousWorldSpaceCameraPos = motionVectorsPersistentData.previousWorldSpaceCameraPos;
				Vector3 previousPreviousWorldSpaceCameraPos = motionVectorsPersistentData.previousPreviousWorldSpaceCameraPos;
				perViewConfig.currentView.SetColumn(3, new Vector4(-worldSpaceCameraPos.x, -worldSpaceCameraPos.y, -worldSpaceCameraPos.z, 1f));
				perViewConfig.lastView.SetColumn(3, new Vector4(-previousWorldSpaceCameraPos.x, -previousWorldSpaceCameraPos.y, -previousWorldSpaceCameraPos.z, 1f));
				perViewConfig.lastLastView.SetColumn(3, new Vector4(-previousPreviousWorldSpaceCameraPos.x, -previousPreviousWorldSpaceCameraPos.y, -previousPreviousWorldSpaceCameraPos.z, 1f));
				STP.perViewConfigs[i] = perViewConfig;
			}
			config.numActiveViews = num2;
			config.perViewConfigs = STP.perViewConfigs;
		}

		internal static void Execute(RenderGraph renderGraph, UniversalResourceData resourceData, UniversalCameraData cameraData, TextureHandle inputColor, TextureHandle inputDepth, TextureHandle inputMotion, TextureHandle destination, Texture2D noiseTexture)
		{
			TextureHandle textureHandle = TextureHandle.nullHandle;
			int debugViewIndex = 0;
			DebugHandler activeDebugHandler = ScriptableRenderPass.GetActiveDebugHandler(cameraData);
			DebugFullScreenMode debugFullScreenMode;
			if (activeDebugHandler != null && activeDebugHandler.TryGetFullscreenDebugMode(out debugFullScreenMode) && debugFullScreenMode == DebugFullScreenMode.STP)
			{
				TextureDesc textureDesc = new TextureDesc(cameraData.pixelWidth, cameraData.pixelHeight, false, cameraData.xr.enabled && cameraData.xr.singlePassEnabled);
				textureDesc.name = "STP Debug View";
				textureDesc.format = GraphicsFormat.R8G8B8A8_UNorm;
				textureDesc.clearBuffer = true;
				textureDesc.enableRandomWrite = true;
				textureHandle = renderGraph.CreateTexture(textureDesc);
				debugViewIndex = activeDebugHandler.stpDebugViewIndex;
				resourceData.stpDebugView = textureHandle;
			}
			STP.Config config;
			StpUtils.PopulateStpConfig(cameraData, inputColor, inputDepth, inputMotion, debugViewIndex, textureHandle, destination, noiseTexture, out config);
			STP.Execute(renderGraph, ref config);
		}

		internal static TemporalAA.JitterFunc s_JitterFunc = new TemporalAA.JitterFunc(StpUtils.CalculateJitter);
	}
}
