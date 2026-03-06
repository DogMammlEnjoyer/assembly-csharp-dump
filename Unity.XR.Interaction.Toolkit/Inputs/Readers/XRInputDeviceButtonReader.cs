using System;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
	[AddComponentMenu("XR/Input/XR Input Device Button Reader", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Inputs.Readers.XRInputDeviceButtonReader.html")]
	[DefaultExecutionOrder(-31000)]
	public sealed class XRInputDeviceButtonReader : MonoBehaviour, IXRInputButtonReader, IXRInputValueReader<float>, IXRInputValueReader
	{
		public XRInputDeviceBoolValueReader boolValueReader
		{
			get
			{
				return this.m_BoolValueReader;
			}
			set
			{
				this.m_BoolValueReader = value;
			}
		}

		public XRInputDeviceFloatValueReader floatValueReader
		{
			get
			{
				return this.m_FloatValueReader;
			}
			set
			{
				this.m_FloatValueReader = value;
			}
		}

		private void Awake()
		{
			if (this.m_BoolValueReader == null)
			{
				Debug.LogError("No bool value reader set for XRInputDeviceButtonReader.", this);
			}
			if (this.m_FloatValueReader == null)
			{
				Debug.LogError("No float value reader set for XRInputDeviceButtonReader.", this);
			}
		}

		private void Update()
		{
			bool isPerformed = this.m_IsPerformed;
			XRInputDeviceBoolValueReader xrinputDeviceBoolValueReader;
			this.m_IsPerformed = (this.TryGetBoolValueReader(out xrinputDeviceBoolValueReader) && xrinputDeviceBoolValueReader.ReadValue());
			this.m_WasPerformedThisFrame = (!isPerformed && this.m_IsPerformed);
			this.m_WasCompletedThisFrame = (isPerformed && !this.m_IsPerformed);
		}

		public bool ReadIsPerformed()
		{
			return this.m_IsPerformed;
		}

		public bool ReadWasPerformedThisFrame()
		{
			return this.m_WasPerformedThisFrame;
		}

		public bool ReadWasCompletedThisFrame()
		{
			return this.m_WasCompletedThisFrame;
		}

		public float ReadValue()
		{
			XRInputDeviceFloatValueReader xrinputDeviceFloatValueReader;
			if (this.TryGetFloatValueReader(out xrinputDeviceFloatValueReader))
			{
				return xrinputDeviceFloatValueReader.ReadValue();
			}
			return 0f;
		}

		public bool TryReadValue(out float value)
		{
			XRInputDeviceFloatValueReader xrinputDeviceFloatValueReader;
			if (this.TryGetFloatValueReader(out xrinputDeviceFloatValueReader))
			{
				return xrinputDeviceFloatValueReader.TryReadValue(out value);
			}
			value = 0f;
			return false;
		}

		private bool TryGetBoolValueReader(out XRInputDeviceBoolValueReader reference)
		{
			return this.m_BoolValueReaderCache.TryGet(this.m_BoolValueReader, out reference);
		}

		private bool TryGetFloatValueReader(out XRInputDeviceFloatValueReader reference)
		{
			return this.m_FloatValueReaderCache.TryGet(this.m_FloatValueReader, out reference);
		}

		[SerializeField]
		[Tooltip("The value that is read to determine whether the button is down.")]
		private XRInputDeviceBoolValueReader m_BoolValueReader;

		[SerializeField]
		[Tooltip("The value that is read to determine the scalar value that varies from 0 to 1.")]
		private XRInputDeviceFloatValueReader m_FloatValueReader;

		private bool m_IsPerformed;

		private bool m_WasPerformedThisFrame;

		private bool m_WasCompletedThisFrame;

		private readonly UnityObjectReferenceCache<XRInputDeviceBoolValueReader> m_BoolValueReaderCache = new UnityObjectReferenceCache<XRInputDeviceBoolValueReader>();

		private readonly UnityObjectReferenceCache<XRInputDeviceFloatValueReader> m_FloatValueReaderCache = new UnityObjectReferenceCache<XRInputDeviceFloatValueReader>();
	}
}
