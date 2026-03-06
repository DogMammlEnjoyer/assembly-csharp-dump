using System;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
	public static class XRInputReaderUtility
	{
		public static void SetInputProperty(ref XRInputHapticImpulseProvider property, XRInputHapticImpulseProvider value, Behaviour behavior)
		{
			if (value == null)
			{
				Debug.LogError("Setting XRInputHapticImpulseProvider property to null is disallowed and has therefore been ignored.", behavior);
				return;
			}
			if (Application.isPlaying)
			{
				XRInputHapticImpulseProvider xrinputHapticImpulseProvider = property;
				if (xrinputHapticImpulseProvider != null)
				{
					xrinputHapticImpulseProvider.DisableDirectActionIfModeUsed();
				}
			}
			property = value;
			if (Application.isPlaying && behavior.isActiveAndEnabled)
			{
				property.EnableDirectActionIfModeUsed();
			}
		}

		public static void SetInputProperty(ref XRInputButtonReader property, XRInputButtonReader value, Behaviour behavior)
		{
			if (value == null)
			{
				Debug.LogError("Setting XRInputButtonReader property to null is disallowed and has therefore been ignored.", behavior);
				return;
			}
			if (Application.isPlaying)
			{
				XRInputButtonReader xrinputButtonReader = property;
				if (xrinputButtonReader != null)
				{
					xrinputButtonReader.DisableDirectActionIfModeUsed();
				}
			}
			property = value;
			if (Application.isPlaying && behavior.isActiveAndEnabled)
			{
				property.EnableDirectActionIfModeUsed();
			}
		}

		public static void SetInputProperty<TValue>(ref XRInputValueReader<TValue> property, XRInputValueReader<TValue> value, Behaviour behavior) where TValue : struct
		{
			if (value == null)
			{
				Debug.LogError("Setting XRInputValueReader property to null is disallowed and has therefore been ignored.", behavior);
				return;
			}
			if (Application.isPlaying)
			{
				XRInputValueReader<TValue> xrinputValueReader = property;
				if (xrinputValueReader != null)
				{
					xrinputValueReader.DisableDirectActionIfModeUsed();
				}
			}
			property = value;
			if (Application.isPlaying && behavior.isActiveAndEnabled)
			{
				property.EnableDirectActionIfModeUsed();
			}
		}

		internal static void SetInputProperty(ref XRInputButtonReader property, XRInputButtonReader value, Behaviour behavior, List<XRInputButtonReader> buttonReaders)
		{
			if (value == null)
			{
				Debug.LogError("Setting XRInputButtonReader property to null is disallowed and has therefore been ignored.", behavior);
				return;
			}
			if (Application.isPlaying && property != null)
			{
				if (buttonReaders != null)
				{
					buttonReaders.Remove(property);
				}
				property.DisableDirectActionIfModeUsed();
			}
			property = value;
			if (Application.isPlaying)
			{
				if (buttonReaders != null)
				{
					buttonReaders.Add(property);
				}
				if (behavior.isActiveAndEnabled)
				{
					property.EnableDirectActionIfModeUsed();
				}
			}
		}

		internal static void SetInputProperty<TValue>(ref XRInputValueReader<TValue> property, XRInputValueReader<TValue> value, Behaviour behavior, List<XRInputValueReader> valueReaders) where TValue : struct
		{
			if (value == null)
			{
				Debug.LogError("Setting XRInputValueReader property to null is disallowed and has therefore been ignored.", behavior);
				return;
			}
			if (Application.isPlaying && property != null)
			{
				if (valueReaders != null)
				{
					valueReaders.Remove(property);
				}
				property.DisableDirectActionIfModeUsed();
			}
			property = value;
			if (Application.isPlaying)
			{
				if (valueReaders != null)
				{
					valueReaders.Add(property);
				}
				if (behavior.isActiveAndEnabled)
				{
					property.EnableDirectActionIfModeUsed();
				}
			}
		}
	}
}
