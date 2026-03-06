using System;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.Cinemachine
{
	public static class CinemachineCore
	{
		internal static float CurrentUnscaledTime
		{
			get
			{
				if (CinemachineCore.CurrentUnscaledTimeTimeOverride < 0f)
				{
					return Time.unscaledTime;
				}
				return CinemachineCore.CurrentUnscaledTimeTimeOverride;
			}
		}

		internal static Color SoloGUIColor()
		{
			return Color.Lerp(Color.red, Color.yellow, 0.8f);
		}

		public static float DeltaTime
		{
			get
			{
				if (CinemachineCore.UniformDeltaTimeOverride < 0f)
				{
					return Time.deltaTime;
				}
				return CinemachineCore.UniformDeltaTimeOverride;
			}
		}

		public static float CurrentTime
		{
			get
			{
				if (CinemachineCore.CurrentTimeOverride < 0f)
				{
					return Time.time;
				}
				return CinemachineCore.CurrentTimeOverride;
			}
		}

		public static int CurrentUpdateFrame { get; internal set; }

		public static int VirtualCameraCount
		{
			get
			{
				return CameraUpdateManager.VirtualCameraCount;
			}
		}

		public static CinemachineVirtualCameraBase GetVirtualCamera(int index)
		{
			return CameraUpdateManager.GetVirtualCamera(index);
		}

		public static ICinemachineCamera SoloCamera
		{
			get
			{
				return CinemachineCore.s_SoloCamera;
			}
			set
			{
				if (value != null && !CinemachineCore.IsLive(value))
				{
					value.OnCameraActivated(new ICinemachineCamera.ActivationEventParams
					{
						Origin = null,
						OutgoingCamera = null,
						IncomingCamera = value,
						IsCut = true,
						WorldUp = Vector3.up,
						DeltaTime = CinemachineCore.DeltaTime
					});
				}
				CinemachineCore.s_SoloCamera = value;
			}
		}

		public static bool IsLive(ICinemachineCamera vcam)
		{
			if (vcam != null)
			{
				int activeBrainCount = CinemachineBrain.ActiveBrainCount;
				for (int i = 0; i < activeBrainCount; i++)
				{
					CinemachineBrain activeBrain = CinemachineBrain.GetActiveBrain(i);
					if (activeBrain != null && activeBrain.IsLiveChild(vcam, false))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool IsLiveInBlend(ICinemachineCamera vcam)
		{
			if (vcam != null)
			{
				int activeBrainCount = CinemachineBrain.ActiveBrainCount;
				for (int i = 0; i < activeBrainCount; i++)
				{
					CinemachineBrain activeBrain = CinemachineBrain.GetActiveBrain(i);
					if (activeBrain != null && activeBrain.IsLiveInBlend(vcam))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static CinemachineBrain FindPotentialTargetBrain(CinemachineVirtualCameraBase vcam)
		{
			if (vcam != null)
			{
				int activeBrainCount = CinemachineBrain.ActiveBrainCount;
				for (int i = 0; i < activeBrainCount; i++)
				{
					CinemachineBrain activeBrain = CinemachineBrain.GetActiveBrain(i);
					if (activeBrain != null && activeBrain.OutputCamera != null && activeBrain.IsLiveChild(vcam, false))
					{
						return activeBrain;
					}
				}
				uint outputChannel = (uint)vcam.OutputChannel;
				for (int j = 0; j < activeBrainCount; j++)
				{
					CinemachineBrain activeBrain2 = CinemachineBrain.GetActiveBrain(j);
					if (activeBrain2 != null && activeBrain2.OutputCamera != null && (activeBrain2.ChannelMask & (OutputChannels)outputChannel) != (OutputChannels)0)
					{
						return activeBrain2;
					}
				}
			}
			return null;
		}

		public static void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
		{
			int virtualCameraCount = CameraUpdateManager.VirtualCameraCount;
			for (int i = 0; i < virtualCameraCount; i++)
			{
				CinemachineCore.GetVirtualCamera(i).OnTargetObjectWarped(target, positionDelta);
			}
		}

		public static void ResetCameraState()
		{
			int virtualCameraCount = CameraUpdateManager.VirtualCameraCount;
			for (int i = 0; i < virtualCameraCount; i++)
			{
				CinemachineCore.GetVirtualCamera(i).PreviousStateIsValid = false;
			}
			int activeBrainCount = CinemachineBrain.ActiveBrainCount;
			for (int j = 0; j < activeBrainCount; j++)
			{
				CinemachineBrain.GetActiveBrain(j).ResetState();
			}
		}

		internal const int kStreamingVersion = 20241001;

		public const string kPackageRoot = "Packages/com.unity.cinemachine";

		internal static float CurrentUnscaledTimeTimeOverride = -1f;

		internal static bool UnitTestMode = false;

		public static CinemachineCore.AxisInputDelegate GetInputAxis = (string <p0>) => 0f;

		public static float UniformDeltaTimeOverride = -1f;

		public static float CurrentTimeOverride = -1f;

		public static CinemachineCore.GetBlendOverrideDelegate GetBlendOverride;

		public static CinemachineCore.GetCustomBlenderDelegate GetCustomBlender;

		public static CinemachineCore.BrainEvent CameraUpdatedEvent = new CinemachineCore.BrainEvent();

		public static ICinemachineCamera.ActivationEvent CameraActivatedEvent = new ICinemachineCamera.ActivationEvent();

		public static CinemachineCore.CameraEvent CameraDeactivatedEvent = new CinemachineCore.CameraEvent();

		public static CinemachineCore.BlendEvent BlendCreatedEvent = new CinemachineCore.BlendEvent();

		public static CinemachineCore.CameraEvent BlendFinishedEvent = new CinemachineCore.CameraEvent();

		private static ICinemachineCamera s_SoloCamera;

		public enum Stage
		{
			Body,
			Aim,
			Noise,
			Finalize
		}

		[Flags]
		public enum BlendHints
		{
			SphericalPosition = 1,
			CylindricalPosition = 2,
			ScreenSpaceAimWhenTargetsDiffer = 4,
			InheritPosition = 8,
			IgnoreTarget = 16,
			FreezeWhenBlendingOut = 32
		}

		public delegate float AxisInputDelegate(string axisName);

		public delegate CinemachineBlendDefinition GetBlendOverrideDelegate(ICinemachineCamera fromVcam, ICinemachineCamera toVcam, CinemachineBlendDefinition defaultBlend, Object owner);

		public delegate CinemachineBlend.IBlender GetCustomBlenderDelegate(ICinemachineCamera fromCam, ICinemachineCamera toCam);

		[Serializable]
		public class CameraEvent : UnityEvent<ICinemachineMixer, ICinemachineCamera>
		{
		}

		[Serializable]
		public class BrainEvent : UnityEvent<CinemachineBrain>
		{
		}

		public struct BlendEventParams
		{
			public ICinemachineMixer Origin;

			public CinemachineBlend Blend;
		}

		[Serializable]
		public class BlendEvent : UnityEvent<CinemachineCore.BlendEventParams>
		{
		}
	}
}
