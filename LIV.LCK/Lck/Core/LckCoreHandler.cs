using System;
using Liv.Lck.Settings;
using UnityEngine;
using UnityEngine.Rendering;

namespace Liv.Lck.Core
{
	public static class LckCoreHandler
	{
		internal static Result<bool> LckCoreInitializationResult { get; private set; }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void Initialize()
		{
			LckCoreHandler.InitializeInternal();
		}

		private static void InitializeInternal()
		{
			LckSettings instance = LckSettings.Instance;
			LckCore.SetMaxLogLevel(instance.CoreLogLevel);
			if (instance.CoreLogLevel == LevelFilter.Info)
			{
				Debug.Log("LCK Core Handler initializing...");
			}
			GameInfo gameInfo = new GameInfo
			{
				GameName = instance.GameName,
				GameVersion = Application.version,
				ProjectName = Application.productName,
				CompanyName = Application.companyName,
				EngineVersion = Application.unityVersion,
				RenderPipeline = LckCoreHandler.GetRenderPipelineType(),
				GraphicsAPI = SystemInfo.graphicsDeviceType.ToString(),
				Platform = Application.platform.ToString(),
				PersistentDataPath = Application.persistentDataPath
			};
			LckInfo lckInfo = new LckInfo
			{
				Version = "1.4.3",
				BuildNumber = -1,
				ForceStagingEnvironment = LckSettings.Instance.ForceStagingEnvironment
			};
			LckCoreHandler.LckCoreInitializationResult = LckCore.Initialize(instance.TrackingId, gameInfo, lckInfo);
			if (LckCoreHandler.LckCoreInitializationResult.IsOk)
			{
				return;
			}
			CoreError? err = LckCoreHandler.LckCoreInitializationResult.Err;
			CoreError coreError = CoreError.MissingTrackingId;
			if (err.GetValueOrDefault() == coreError & err != null)
			{
				Debug.LogError("LCK: Missing or bad Tracking ID supplied. Recording and streaming will not be available.");
				return;
			}
			Debug.LogError(string.Format("LCK: LCK Core initialization failed: {0} - {1}", LckCoreHandler.LckCoreInitializationResult.Err, LckCoreHandler.LckCoreInitializationResult.Message));
		}

		private static string GetRenderPipelineType()
		{
			if (!GraphicsSettings.defaultRenderPipeline)
			{
				return "Built-in render pipeline";
			}
			if (GraphicsSettings.defaultRenderPipeline.GetType().ToString().Contains("HDRenderPipelineAsset"))
			{
				return "High Definition render pipeline";
			}
			if (GraphicsSettings.defaultRenderPipeline.GetType().ToString().Contains("UniversalRenderPipelineAsset"))
			{
				return "Universal render pipeline";
			}
			return "Custom render pipeline";
		}
	}
}
