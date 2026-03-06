using System;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
	public abstract class XRInputDeviceValueReader<TValue> : XRInputDeviceValueReader, IXRInputValueReader<TValue>, IXRInputValueReader where TValue : struct
	{
		public InputFeatureUsageString<TValue> usage
		{
			get
			{
				return this.m_Usage;
			}
			set
			{
				this.m_Usage = value;
			}
		}

		public abstract TValue ReadValue();

		public abstract bool TryReadValue(out TValue value);

		protected bool ReadBoolValue()
		{
			bool flag;
			return this.RefreshInputDeviceIfNeeded() && this.m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<bool>(this.m_Usage.name), out flag) && flag;
		}

		protected uint ReadUIntValue()
		{
			uint result;
			if (this.RefreshInputDeviceIfNeeded() && this.m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<uint>(this.m_Usage.name), out result))
			{
				return result;
			}
			return 0U;
		}

		protected float ReadFloatValue()
		{
			float result;
			if (this.RefreshInputDeviceIfNeeded() && this.m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<float>(this.m_Usage.name), out result))
			{
				return result;
			}
			return 0f;
		}

		protected Vector2 ReadVector2Value()
		{
			Vector2 result;
			if (this.RefreshInputDeviceIfNeeded() && this.m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<Vector2>(this.m_Usage.name), out result))
			{
				return result;
			}
			return default(Vector2);
		}

		protected Vector3 ReadVector3Value()
		{
			Vector3 result;
			if (this.RefreshInputDeviceIfNeeded() && this.m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<Vector3>(this.m_Usage.name), out result))
			{
				return result;
			}
			return default(Vector3);
		}

		protected Quaternion ReadQuaternionValue()
		{
			Quaternion result;
			if (this.RefreshInputDeviceIfNeeded() && this.m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<Quaternion>(this.m_Usage.name), out result))
			{
				return result;
			}
			return default(Quaternion);
		}

		protected InputTrackingState ReadInputTrackingStateValue()
		{
			InputTrackingState result;
			if (this.RefreshInputDeviceIfNeeded() && this.m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<InputTrackingState>(this.m_Usage.name), out result))
			{
				return result;
			}
			return InputTrackingState.None;
		}

		protected bool TryReadBoolValue(out bool value)
		{
			if (this.RefreshInputDeviceIfNeeded() && this.m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<bool>(this.m_Usage.name), out value))
			{
				return true;
			}
			value = false;
			return false;
		}

		protected bool TryReadUIntValue(out uint value)
		{
			if (this.RefreshInputDeviceIfNeeded() && this.m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<uint>(this.m_Usage.name), out value))
			{
				return true;
			}
			value = 0U;
			return false;
		}

		protected bool TryReadFloatValue(out float value)
		{
			if (this.RefreshInputDeviceIfNeeded() && this.m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<float>(this.m_Usage.name), out value))
			{
				return true;
			}
			value = 0f;
			return false;
		}

		protected bool TryReadVector2Value(out Vector2 value)
		{
			if (this.RefreshInputDeviceIfNeeded() && this.m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<Vector2>(this.m_Usage.name), out value))
			{
				return true;
			}
			value = default(Vector2);
			return false;
		}

		protected bool TryReadVector3Value(out Vector3 value)
		{
			if (this.RefreshInputDeviceIfNeeded() && this.m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<Vector3>(this.m_Usage.name), out value))
			{
				return true;
			}
			value = default(Vector3);
			return false;
		}

		protected bool TryReadQuaternionValue(out Quaternion value)
		{
			if (this.RefreshInputDeviceIfNeeded() && this.m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<Quaternion>(this.m_Usage.name), out value))
			{
				return true;
			}
			value = default(Quaternion);
			return false;
		}

		protected bool TryReadInputTrackingStateValue(out InputTrackingState value)
		{
			if (this.RefreshInputDeviceIfNeeded() && this.m_InputDevice.TryGetFeatureValue(new InputFeatureUsage<InputTrackingState>(this.m_Usage.name), out value))
			{
				return true;
			}
			value = InputTrackingState.None;
			return false;
		}

		protected bool RefreshInputDeviceIfNeeded()
		{
			return this.m_InputDevice.isValid || XRInputTrackingAggregator.TryGetDeviceWithExactCharacteristics(this.m_Characteristics, out this.m_InputDevice);
		}

		[SerializeField]
		[Tooltip("The name of the input feature to read.")]
		private InputFeatureUsageString<TValue> m_Usage;

		private InputDevice m_InputDevice;
	}
}
