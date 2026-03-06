using System;
using System.Diagnostics;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.EnhancedTouch
{
	public static class EnhancedTouchSupport
	{
		public static bool enabled
		{
			get
			{
				return EnhancedTouchSupport.s_Enabled > 0;
			}
		}

		public static void Enable()
		{
			EnhancedTouchSupport.s_Enabled++;
			if (EnhancedTouchSupport.s_Enabled > 1)
			{
				return;
			}
			InputSystem.onDeviceChange += EnhancedTouchSupport.OnDeviceChange;
			InputSystem.onBeforeUpdate += Touch.BeginUpdate;
			InputSystem.onSettingsChange += EnhancedTouchSupport.OnSettingsChange;
			EnhancedTouchSupport.SetUpState();
		}

		public static void Disable()
		{
			if (!EnhancedTouchSupport.enabled)
			{
				return;
			}
			EnhancedTouchSupport.s_Enabled--;
			if (EnhancedTouchSupport.s_Enabled > 0)
			{
				return;
			}
			InputSystem.onDeviceChange -= EnhancedTouchSupport.OnDeviceChange;
			InputSystem.onBeforeUpdate -= Touch.BeginUpdate;
			InputSystem.onSettingsChange -= EnhancedTouchSupport.OnSettingsChange;
			EnhancedTouchSupport.TearDownState();
		}

		internal static void Reset()
		{
			Touch.s_GlobalState.touchscreens = default(InlinedArray<Touchscreen>);
			Touch.s_GlobalState.playerState.Destroy();
			Touch.s_GlobalState.playerState = default(Touch.FingerAndTouchState);
			EnhancedTouchSupport.s_Enabled = 0;
		}

		private static void SetUpState()
		{
			Touch.s_GlobalState.playerState.updateMask = (InputUpdateType.Dynamic | InputUpdateType.Fixed | InputUpdateType.Manual);
			EnhancedTouchSupport.s_UpdateMode = InputSystem.settings.updateMode;
			foreach (InputDevice device in InputSystem.devices)
			{
				EnhancedTouchSupport.OnDeviceChange(device, InputDeviceChange.Added);
			}
		}

		internal static void TearDownState()
		{
			foreach (InputDevice device in InputSystem.devices)
			{
				EnhancedTouchSupport.OnDeviceChange(device, InputDeviceChange.Removed);
			}
			Touch.s_GlobalState.playerState.Destroy();
			Touch.s_GlobalState.playerState = default(Touch.FingerAndTouchState);
		}

		private static void OnDeviceChange(InputDevice device, InputDeviceChange change)
		{
			if (change != InputDeviceChange.Added)
			{
				if (change != InputDeviceChange.Removed)
				{
					return;
				}
				Touchscreen touchscreen = device as Touchscreen;
				if (touchscreen != null)
				{
					Touch.RemoveTouchscreen(touchscreen);
				}
			}
			else
			{
				Touchscreen touchscreen2 = device as Touchscreen;
				if (touchscreen2 != null)
				{
					Touch.AddTouchscreen(touchscreen2);
					return;
				}
			}
		}

		private static void OnSettingsChange()
		{
			InputSettings.UpdateMode updateMode = InputSystem.settings.updateMode;
			if (EnhancedTouchSupport.s_UpdateMode == updateMode)
			{
				return;
			}
			EnhancedTouchSupport.TearDownState();
			EnhancedTouchSupport.SetUpState();
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		internal static void CheckEnabled()
		{
			if (!EnhancedTouchSupport.enabled)
			{
				throw new InvalidOperationException("EnhancedTouch API is not enabled; call EnhancedTouchSupport.Enable()");
			}
		}

		private static int s_Enabled;

		private static InputSettings.UpdateMode s_UpdateMode;
	}
}
