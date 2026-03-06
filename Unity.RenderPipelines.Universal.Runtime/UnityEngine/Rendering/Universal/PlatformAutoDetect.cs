using System;

namespace UnityEngine.Rendering.Universal
{
	internal static class PlatformAutoDetect
	{
		internal static void Initialize()
		{
			PlatformAutoDetect.isXRMobile = false;
			PlatformAutoDetect.isShaderAPIMobileDefined = GraphicsSettings.HasShaderDefine(BuiltinShaderDefine.SHADER_API_MOBILE);
			PlatformAutoDetect.isSwitch = (Application.platform == RuntimePlatform.Switch);
		}

		internal static bool isXRMobile { get; private set; } = false;

		internal static bool isShaderAPIMobileDefined { get; private set; } = false;

		internal static bool isSwitch { get; private set; } = false;

		internal static ShEvalMode ShAutoDetect(ShEvalMode mode)
		{
			if (mode != ShEvalMode.Auto)
			{
				return mode;
			}
			if (PlatformAutoDetect.isXRMobile || PlatformAutoDetect.isShaderAPIMobileDefined || PlatformAutoDetect.isSwitch)
			{
				return ShEvalMode.PerVertex;
			}
			return ShEvalMode.PerPixel;
		}

		internal static bool isRunningOnPowerVRGPU = SystemInfo.graphicsDeviceName.Contains("PowerVR");
	}
}
