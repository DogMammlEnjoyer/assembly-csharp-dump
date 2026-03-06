using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics.OpenXR;
using UnityEngine.XR.OpenXR.Input;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
	public class HapticControlActionManager
	{
		public HapticControlActionManager()
		{
			this.m_DeviceChannelGroup = new HapticImpulseCommandChannelGroup();
			this.m_OpenXRChannel = new OpenXRHapticImpulseChannel();
			this.m_OpenXRChannelGroup = new HapticImpulseSingleChannelGroup(this.m_OpenXRChannel);
		}

		public IXRHapticImpulseChannelGroup GetChannelGroup(InputAction action)
		{
			if (action == null)
			{
				return null;
			}
			InputControl activeControl = action.activeControl;
			if (activeControl == null)
			{
				ReadOnlyArray<InputControl> controls = action.controls;
				if (controls.Count > 0)
				{
					HapticControl hapticControl = controls[0] as HapticControl;
					if (hapticControl != null)
					{
						this.m_OpenXRChannel.hapticAction = action;
						this.m_OpenXRChannel.device = hapticControl.device;
						return this.m_OpenXRChannelGroup;
					}
				}
				return null;
			}
			this.m_DeviceChannelGroup.Initialize(activeControl.device);
			return this.m_DeviceChannelGroup;
		}

		private readonly HapticImpulseCommandChannelGroup m_DeviceChannelGroup;

		private readonly OpenXRHapticImpulseChannel m_OpenXRChannel;

		private readonly HapticImpulseSingleChannelGroup m_OpenXRChannelGroup;
	}
}
