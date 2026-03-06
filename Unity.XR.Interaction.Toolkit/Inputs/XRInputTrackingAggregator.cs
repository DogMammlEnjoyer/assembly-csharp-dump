using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.OpenXR.Features.Interactions;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs
{
	public static class XRInputTrackingAggregator
	{
		public static TrackingStatus GetHMDStatus()
		{
			if (!Application.isPlaying)
			{
				return default(TrackingStatus);
			}
			XRHMD device = InputSystem.GetDevice<XRHMD>();
			if (device != null)
			{
				return XRInputTrackingAggregator.GetTrackingStatus(device);
			}
			InputDevice device2;
			if (XRInputTrackingAggregator.TryGetDeviceWithExactCharacteristics(XRInputTrackingAggregator.Characteristics.hmd, out device2))
			{
				return XRInputTrackingAggregator.GetTrackingStatus(device2);
			}
			return default(TrackingStatus);
		}

		public static TrackingStatus GetEyeGazeStatus()
		{
			if (!Application.isPlaying)
			{
				return default(TrackingStatus);
			}
			EyeGazeInteraction.EyeGazeDevice device = InputSystem.GetDevice<EyeGazeInteraction.EyeGazeDevice>();
			if (device != null)
			{
				return XRInputTrackingAggregator.GetTrackingStatus(device);
			}
			InputDevice device2;
			if (XRInputTrackingAggregator.TryGetDeviceWithExactCharacteristics(XRInputTrackingAggregator.Characteristics.eyeGaze, out device2))
			{
				return XRInputTrackingAggregator.GetTrackingStatus(device2);
			}
			return default(TrackingStatus);
		}

		public static TrackingStatus GetLeftControllerStatus()
		{
			if (!Application.isPlaying)
			{
				return default(TrackingStatus);
			}
			XRController device = InputSystem.GetDevice<XRController>(CommonUsages.LeftHand);
			if (device != null)
			{
				return XRInputTrackingAggregator.GetTrackingStatus(device);
			}
			InputDevice device2;
			if (XRInputTrackingAggregator.TryGetDeviceWithExactCharacteristics(XRInputTrackingAggregator.Characteristics.leftController, out device2))
			{
				return XRInputTrackingAggregator.GetTrackingStatus(device2);
			}
			return default(TrackingStatus);
		}

		public static TrackingStatus GetRightControllerStatus()
		{
			if (!Application.isPlaying)
			{
				return default(TrackingStatus);
			}
			XRController device = InputSystem.GetDevice<XRController>(CommonUsages.RightHand);
			if (device != null)
			{
				return XRInputTrackingAggregator.GetTrackingStatus(device);
			}
			InputDevice device2;
			if (XRInputTrackingAggregator.TryGetDeviceWithExactCharacteristics(XRInputTrackingAggregator.Characteristics.rightController, out device2))
			{
				return XRInputTrackingAggregator.GetTrackingStatus(device2);
			}
			return default(TrackingStatus);
		}

		public static TrackingStatus GetLeftTrackedHandStatus()
		{
			if (!Application.isPlaying)
			{
				return default(TrackingStatus);
			}
			InputDevice device;
			if (XRInputTrackingAggregator.TryGetDeviceWithExactCharacteristics(XRInputTrackingAggregator.Characteristics.leftTrackedHand, out device))
			{
				return XRInputTrackingAggregator.GetTrackingStatus(device);
			}
			return default(TrackingStatus);
		}

		public static TrackingStatus GetRightTrackedHandStatus()
		{
			if (!Application.isPlaying)
			{
				return default(TrackingStatus);
			}
			InputDevice device;
			if (XRInputTrackingAggregator.TryGetDeviceWithExactCharacteristics(XRInputTrackingAggregator.Characteristics.rightTrackedHand, out device))
			{
				return XRInputTrackingAggregator.GetTrackingStatus(device);
			}
			return default(TrackingStatus);
		}

		public static TrackingStatus GetLeftMetaAimHandStatus()
		{
			bool isPlaying = Application.isPlaying;
			return default(TrackingStatus);
		}

		public static TrackingStatus GetRightMetaAimHandStatus()
		{
			bool isPlaying = Application.isPlaying;
			return default(TrackingStatus);
		}

		internal static bool TryGetDeviceWithExactCharacteristics(InputDeviceCharacteristics desiredCharacteristics, out InputDevice inputDevice)
		{
			if (XRInputTrackingAggregator.s_XRInputDevices == null)
			{
				XRInputTrackingAggregator.s_XRInputDevices = new List<InputDevice>();
			}
			InputDevices.GetDevices(XRInputTrackingAggregator.s_XRInputDevices);
			for (int i = 0; i < XRInputTrackingAggregator.s_XRInputDevices.Count; i++)
			{
				inputDevice = XRInputTrackingAggregator.s_XRInputDevices[i];
				if (inputDevice.characteristics == desiredCharacteristics)
				{
					return true;
				}
			}
			inputDevice = default(InputDevice);
			return false;
		}

		private unsafe static TrackingStatus GetTrackingStatus(TrackedDevice device)
		{
			if (device == null)
			{
				return default(TrackingStatus);
			}
			return new TrackingStatus
			{
				isConnected = device.added,
				isTracked = device.isTracked.isPressed,
				trackingState = (InputTrackingState)(*device.trackingState.value)
			};
		}

		private unsafe static TrackingStatus GetTrackingStatus(EyeGazeInteraction.EyeGazeDevice device)
		{
			if (device == null)
			{
				return default(TrackingStatus);
			}
			return new TrackingStatus
			{
				isConnected = device.added,
				isTracked = device.pose.isTracked.isPressed,
				trackingState = (InputTrackingState)(*device.pose.trackingState.value)
			};
		}

		private static TrackingStatus GetTrackingStatus(InputDevice device)
		{
			bool flag;
			InputTrackingState inputTrackingState;
			return new TrackingStatus
			{
				isConnected = device.isValid,
				isTracked = (device.TryGetFeatureValue(CommonUsages.isTracked, out flag) && flag),
				trackingState = (device.TryGetFeatureValue(CommonUsages.trackingState, out inputTrackingState) ? inputTrackingState : InputTrackingState.None)
			};
		}

		private static List<InputDevice> s_XRInputDevices;

		public static class Characteristics
		{
			public static InputDeviceCharacteristics hmd
			{
				get
				{
					return InputDeviceCharacteristics.HeadMounted | InputDeviceCharacteristics.TrackedDevice;
				}
			}

			public static InputDeviceCharacteristics eyeGaze
			{
				get
				{
					return InputDeviceCharacteristics.HeadMounted | InputDeviceCharacteristics.EyeTracking | InputDeviceCharacteristics.TrackedDevice;
				}
			}

			public static InputDeviceCharacteristics leftController
			{
				get
				{
					return InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left;
				}
			}

			public static InputDeviceCharacteristics rightController
			{
				get
				{
					return InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right;
				}
			}

			public static InputDeviceCharacteristics leftTrackedHand
			{
				get
				{
					return InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Left;
				}
			}

			public static InputDeviceCharacteristics rightTrackedHand
			{
				get
				{
					return InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Right;
				}
			}

			internal static InputDeviceCharacteristics leftHandInteraction
			{
				get
				{
					return InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Left;
				}
			}

			internal static InputDeviceCharacteristics rightHandInteraction
			{
				get
				{
					return InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Right;
				}
			}

			internal static InputDeviceCharacteristics leftMicrosoftHandInteraction
			{
				get
				{
					return InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left;
				}
			}

			internal static InputDeviceCharacteristics rightMicrosoftHandInteraction
			{
				get
				{
					return InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right;
				}
			}
		}
	}
}
