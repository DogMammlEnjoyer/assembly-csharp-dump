using System;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
	[Serializable]
	public class XRInputValueReader<TValue> : XRInputValueReader, IXRInputValueReader<TValue>, IXRInputValueReader where TValue : struct
	{
		public TValue manualValue
		{
			get
			{
				return this.m_ManualValue;
			}
			set
			{
				this.m_ManualValue = value;
			}
		}

		public IXRInputValueReader<TValue> bypass { get; set; }

		public XRInputValueReader()
		{
		}

		public XRInputValueReader(string name = null, XRInputValueReader.InputSourceMode inputSourceMode = XRInputValueReader.InputSourceMode.InputActionReference) : base(InputActionUtility.CreateValueAction(typeof(TValue), name), inputSourceMode)
		{
		}

		public IXRInputValueReader<TValue> GetObjectReference()
		{
			return this.m_ObjectReference.Get(this.m_ObjectReferenceObject);
		}

		public void SetObjectReference(IXRInputValueReader<TValue> value)
		{
			this.m_ObjectReference.Set(ref this.m_ObjectReferenceObject, value);
		}

		public TValue ReadValue()
		{
			if (this.bypass != null && !this.m_CallingBypass)
			{
				using (new XRInputValueReader<TValue>.BypassScope(this))
				{
					return this.bypass.ReadValue();
				}
			}
			switch (this.m_InputSourceMode)
			{
			default:
				return default(TValue);
			case XRInputValueReader.InputSourceMode.InputAction:
				return XRInputValueReader<TValue>.ReadValue(this.m_InputAction);
			case XRInputValueReader.InputSourceMode.InputActionReference:
			{
				InputActionReference inputActionReference;
				if (!base.TryGetInputActionReference(out inputActionReference))
				{
					return default(TValue);
				}
				return XRInputValueReader<TValue>.ReadValue(inputActionReference.action);
			}
			case XRInputValueReader.InputSourceMode.ObjectReference:
			{
				IXRInputValueReader<TValue> objectReference = this.GetObjectReference();
				if (objectReference == null)
				{
					return default(TValue);
				}
				return objectReference.ReadValue();
			}
			case XRInputValueReader.InputSourceMode.ManualValue:
				return this.m_ManualValue;
			}
		}

		public bool TryReadValue(out TValue value)
		{
			if (this.bypass != null && !this.m_CallingBypass)
			{
				using (new XRInputValueReader<TValue>.BypassScope(this))
				{
					return this.bypass.TryReadValue(out value);
				}
			}
			switch (base.inputSourceMode)
			{
			default:
				value = default(TValue);
				return false;
			case XRInputValueReader.InputSourceMode.InputAction:
				return XRInputValueReader<TValue>.TryReadValue(this.m_InputAction, out value);
			case XRInputValueReader.InputSourceMode.InputActionReference:
			{
				InputActionReference inputActionReference;
				if (base.TryGetInputActionReference(out inputActionReference))
				{
					return XRInputValueReader<TValue>.TryReadValue(inputActionReference.action, out value);
				}
				value = default(TValue);
				return false;
			}
			case XRInputValueReader.InputSourceMode.ObjectReference:
			{
				IXRInputValueReader<TValue> objectReference = this.GetObjectReference();
				if (objectReference != null)
				{
					return objectReference.TryReadValue(out value);
				}
				value = default(TValue);
				return false;
			}
			case XRInputValueReader.InputSourceMode.ManualValue:
				value = this.m_ManualValue;
				return true;
			}
		}

		private static TValue ReadValue(InputAction action)
		{
			if (action == null)
			{
				return default(TValue);
			}
			return action.ReadValue<TValue>();
		}

		private static bool TryReadValue(InputAction action, out TValue value)
		{
			if (action == null)
			{
				value = default(TValue);
				return false;
			}
			value = action.ReadValue<TValue>();
			return action.IsInProgress();
		}

		[SerializeField]
		[RequireInterface(typeof(IXRInputValueReader))]
		private Object m_ObjectReferenceObject;

		[SerializeField]
		private TValue m_ManualValue;

		private bool m_CallingBypass;

		private readonly UnityObjectReferenceCache<IXRInputValueReader<TValue>, Object> m_ObjectReference = new UnityObjectReferenceCache<IXRInputValueReader<TValue>, Object>();

		private readonly struct BypassScope : IDisposable
		{
			public BypassScope(XRInputValueReader<TValue> reader)
			{
				this.m_Reader = reader;
				this.m_Reader.m_CallingBypass = true;
			}

			public void Dispose()
			{
				this.m_Reader.m_CallingBypass = false;
			}

			private readonly XRInputValueReader<TValue> m_Reader;
		}
	}
}
