using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
	public static class HapticsUtility
	{
		public static bool SendHapticImpulse(float amplitude, float duration, HapticsUtility.Controller controller, float frequency = 0f, int channel = 0)
		{
			bool flag = false;
			bool flag2 = false;
			if (controller == HapticsUtility.Controller.Left || controller == HapticsUtility.Controller.Both)
			{
				XRController leftHand = XRController.leftHand;
				if (leftHand != null)
				{
					if (frequency > 0f && channel == 0)
					{
						flag = HapticsUtility.SendHapticImpulseOpenXR(HapticsUtility.GetLeftHapticAction(), amplitude, duration, frequency);
					}
					if (!flag)
					{
						flag = HapticsUtility.SendHapticImpulse(ref HapticsUtility.s_LeftChannelGroup, leftHand, channel, amplitude, duration, frequency);
					}
				}
				else
				{
					flag = HapticsUtility.SendHapticImpulseLegacy(ref HapticsUtility.s_LegacyLeftChannelGroup, ref HapticsUtility.s_LegacyLeftDevice, XRInputTrackingAggregator.Characteristics.leftController, channel, amplitude, duration, frequency);
				}
			}
			if (controller == HapticsUtility.Controller.Right || controller == HapticsUtility.Controller.Both)
			{
				XRController rightHand = XRController.rightHand;
				if (rightHand != null)
				{
					if (frequency > 0f && channel == 0)
					{
						flag2 = HapticsUtility.SendHapticImpulseOpenXR(HapticsUtility.GetRightHapticAction(), amplitude, duration, frequency);
					}
					if (!flag2)
					{
						flag2 = HapticsUtility.SendHapticImpulse(ref HapticsUtility.s_RightChannelGroup, rightHand, channel, amplitude, duration, frequency);
					}
				}
				else
				{
					flag2 = HapticsUtility.SendHapticImpulseLegacy(ref HapticsUtility.s_LegacyRightChannelGroup, ref HapticsUtility.s_LegacyRightDevice, XRInputTrackingAggregator.Characteristics.rightController, channel, amplitude, duration, frequency);
				}
			}
			if (controller == HapticsUtility.Controller.Both)
			{
				return flag && flag2;
			}
			if (controller == HapticsUtility.Controller.Left)
			{
				return flag;
			}
			return controller == HapticsUtility.Controller.Right && flag2;
		}

		private static bool SendHapticImpulseOpenXR(InputAction hapticAction, float amplitude, float duration, float frequency)
		{
			if (HapticsUtility.s_HapticControlManager == null)
			{
				HapticsUtility.s_HapticControlManager = new HapticControlActionManager();
			}
			IXRHapticImpulseChannelGroup channelGroup = HapticsUtility.s_HapticControlManager.GetChannelGroup(hapticAction);
			return channelGroup != null && channelGroup.GetChannel(0).SendHapticImpulse(amplitude, duration, frequency);
		}

		private static bool SendHapticImpulse(ref HapticImpulseCommandChannelGroup channelGroup, InputDevice device, int channel, float amplitude, float duration, float frequency)
		{
			if (channelGroup == null)
			{
				channelGroup = new HapticImpulseCommandChannelGroup();
			}
			channelGroup.Initialize(device);
			IXRHapticImpulseChannel channel2 = channelGroup.GetChannel(channel);
			return channel2 != null && channel2.SendHapticImpulse(amplitude, duration, frequency);
		}

		private static bool SendHapticImpulseLegacy(ref XRInputDeviceHapticImpulseChannelGroup channelGroup, ref InputDevice device, InputDeviceCharacteristics characteristics, int channel, float amplitude, float duration, float frequency)
		{
			if (channelGroup == null)
			{
				channelGroup = new XRInputDeviceHapticImpulseChannelGroup();
			}
			if (device.isValid || XRInputTrackingAggregator.TryGetDeviceWithExactCharacteristics(characteristics, out device))
			{
				channelGroup.Initialize(device);
				IXRHapticImpulseChannel channel2 = channelGroup.GetChannel(channel);
				return channel2 != null && channel2.SendHapticImpulse(amplitude, duration, frequency);
			}
			return false;
		}

		private static InputAction GetLeftHapticAction()
		{
			if (HapticsUtility.s_LeftHapticAction == null)
			{
				HapticsUtility.s_LeftHapticAction = new InputAction("Left Haptic", InputActionType.PassThrough, "<XRController>{LeftHand}/{Haptic}", null, null, null);
				HapticsUtility.s_LeftHapticAction.Enable();
			}
			return HapticsUtility.s_LeftHapticAction;
		}

		private static InputAction GetRightHapticAction()
		{
			if (HapticsUtility.s_RightHapticAction == null)
			{
				HapticsUtility.s_RightHapticAction = new InputAction("Right Haptic", InputActionType.PassThrough, "<XRController>{RightHand}/{Haptic}", null, null, null);
				HapticsUtility.s_RightHapticAction.Enable();
			}
			return HapticsUtility.s_RightHapticAction;
		}

		private static HapticImpulseCommandChannelGroup s_LeftChannelGroup;

		private static HapticImpulseCommandChannelGroup s_RightChannelGroup;

		private static XRInputDeviceHapticImpulseChannelGroup s_LegacyLeftChannelGroup;

		private static XRInputDeviceHapticImpulseChannelGroup s_LegacyRightChannelGroup;

		private static InputDevice s_LegacyLeftDevice;

		private static InputDevice s_LegacyRightDevice;

		private static HapticControlActionManager s_HapticControlManager;

		private static InputAction s_LeftHapticAction;

		private static InputAction s_RightHapticAction;

		public enum Controller
		{
			Left,
			Right,
			Both
		}
	}
}
