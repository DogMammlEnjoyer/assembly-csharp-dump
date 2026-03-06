using System;

namespace UnityEngine.Rendering.Universal
{
	public static class CameraExtensions
	{
		public static UniversalAdditionalCameraData GetUniversalAdditionalCameraData(this Camera camera)
		{
			GameObject gameObject = camera.gameObject;
			UniversalAdditionalCameraData result;
			if (!gameObject.TryGetComponent<UniversalAdditionalCameraData>(out result))
			{
				result = gameObject.AddComponent<UniversalAdditionalCameraData>();
			}
			return result;
		}

		public static VolumeFrameworkUpdateMode GetVolumeFrameworkUpdateMode(this Camera camera)
		{
			return camera.GetUniversalAdditionalCameraData().volumeFrameworkUpdateMode;
		}

		public static void SetVolumeFrameworkUpdateMode(this Camera camera, VolumeFrameworkUpdateMode mode)
		{
			UniversalAdditionalCameraData universalAdditionalCameraData = camera.GetUniversalAdditionalCameraData();
			if (universalAdditionalCameraData.volumeFrameworkUpdateMode == mode)
			{
				return;
			}
			bool requiresVolumeFrameworkUpdate = universalAdditionalCameraData.requiresVolumeFrameworkUpdate;
			universalAdditionalCameraData.volumeFrameworkUpdateMode = mode;
			if (requiresVolumeFrameworkUpdate && !universalAdditionalCameraData.requiresVolumeFrameworkUpdate)
			{
				camera.UpdateVolumeStack(universalAdditionalCameraData);
			}
		}

		public static void UpdateVolumeStack(this Camera camera)
		{
			UniversalAdditionalCameraData universalAdditionalCameraData = camera.GetUniversalAdditionalCameraData();
			camera.UpdateVolumeStack(universalAdditionalCameraData);
		}

		public static void UpdateVolumeStack(this Camera camera, UniversalAdditionalCameraData cameraData)
		{
			if (!VolumeManager.instance.isInitialized)
			{
				Debug.LogError("UpdateVolumeStack must not be called before VolumeManager.instance.Initialize. If you tries calling this from Awake or Start, try instead to use the RenderPipelineManager.activeRenderPipelineCreated callback to be sure your render pipeline is fully initialized before calling this.");
				return;
			}
			if (cameraData.requiresVolumeFrameworkUpdate)
			{
				return;
			}
			if (cameraData.volumeStack == null)
			{
				cameraData.GetOrCreateVolumeStack();
			}
			LayerMask layerMask;
			Transform trigger;
			camera.GetVolumeLayerMaskAndTrigger(cameraData, out layerMask, out trigger);
			VolumeManager.instance.Update(cameraData.volumeStack, trigger, layerMask);
		}

		public static void DestroyVolumeStack(this Camera camera)
		{
			UniversalAdditionalCameraData universalAdditionalCameraData = camera.GetUniversalAdditionalCameraData();
			camera.DestroyVolumeStack(universalAdditionalCameraData);
		}

		public static void DestroyVolumeStack(this Camera camera, UniversalAdditionalCameraData cameraData)
		{
			if (cameraData == null || cameraData.volumeStack == null)
			{
				return;
			}
			cameraData.volumeStack = null;
		}

		internal static void GetVolumeLayerMaskAndTrigger(this Camera camera, UniversalAdditionalCameraData cameraData, out LayerMask layerMask, out Transform trigger)
		{
			layerMask = 1;
			trigger = camera.transform;
			if (cameraData != null)
			{
				layerMask = cameraData.volumeLayerMask;
				trigger = ((cameraData.volumeTrigger != null) ? cameraData.volumeTrigger : trigger);
				return;
			}
			if (camera.cameraType == CameraType.SceneView)
			{
				Camera main = Camera.main;
				UniversalAdditionalCameraData universalAdditionalCameraData = null;
				if (main != null && main.TryGetComponent<UniversalAdditionalCameraData>(out universalAdditionalCameraData))
				{
					layerMask = universalAdditionalCameraData.volumeLayerMask;
				}
				trigger = ((universalAdditionalCameraData != null && universalAdditionalCameraData.volumeTrigger != null) ? universalAdditionalCameraData.volumeTrigger : trigger);
			}
		}
	}
}
