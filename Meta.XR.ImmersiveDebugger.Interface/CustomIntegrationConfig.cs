using System;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger
{
	public static class CustomIntegrationConfig
	{
		public static event CustomIntegrationConfig.GetCameraDelegate GetCameraHandler;

		public static void SetupAllConfig(ICustomIntegrationConfig customConfig)
		{
			CustomIntegrationConfig.GetCameraHandler += customConfig.GetCamera;
		}

		public static void ClearAllConfig(ICustomIntegrationConfig customConfig)
		{
			CustomIntegrationConfig.GetCameraHandler -= customConfig.GetCamera;
		}

		public static Camera GetCamera()
		{
			CustomIntegrationConfig.GetCameraDelegate getCameraHandler = CustomIntegrationConfig.GetCameraHandler;
			if (getCameraHandler == null)
			{
				return null;
			}
			return getCameraHandler();
		}

		public delegate Camera GetCameraDelegate();

		public delegate Transform GetLeftControllerTransformDelegate();

		public delegate Transform GetRightControllerTransformDelegate();
	}
}
