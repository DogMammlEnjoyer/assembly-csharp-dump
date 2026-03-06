using System;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
	[Serializable]
	public class XRInputButtonReader : IXRInputButtonReader, IXRInputValueReader<float>, IXRInputValueReader
	{
		public XRInputButtonReader.InputSourceMode inputSourceMode
		{
			get
			{
				return this.m_InputSourceMode;
			}
			set
			{
				this.m_InputSourceMode = value;
			}
		}

		public InputAction inputActionPerformed
		{
			get
			{
				return this.m_InputActionPerformed;
			}
			set
			{
				this.m_InputActionPerformed = value;
			}
		}

		public InputAction inputActionValue
		{
			get
			{
				return this.m_InputActionValue;
			}
			set
			{
				this.m_InputActionValue = value;
			}
		}

		public InputActionReference inputActionReferencePerformed
		{
			get
			{
				return this.m_InputActionReferencePerformed;
			}
			set
			{
				this.m_InputActionReferencePerformed = value;
			}
		}

		public InputActionReference inputActionReferenceValue
		{
			get
			{
				return this.m_InputActionReferenceValue;
			}
			set
			{
				this.m_InputActionReferenceValue = value;
			}
		}

		public bool manualPerformed
		{
			get
			{
				return this.m_ManualPerformed;
			}
			set
			{
				this.m_ManualPerformed = value;
			}
		}

		public float manualValue
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

		public int manualFramePerformed
		{
			get
			{
				return this.m_ManualFramePerformed;
			}
			set
			{
				this.m_ManualFramePerformed = value;
			}
		}

		public int manualFrameCompleted
		{
			get
			{
				return this.m_ManualFrameCompleted;
			}
			set
			{
				this.m_ManualFrameCompleted = value;
			}
		}

		public IXRInputButtonReader bypass { get; set; }

		public XRInputButtonReader()
		{
		}

		public XRInputButtonReader(string name = null, string valueName = null, bool wantsInitialStateCheck = false, XRInputButtonReader.InputSourceMode inputSourceMode = XRInputButtonReader.InputSourceMode.InputActionReference)
		{
			this.m_InputActionPerformed = InputActionUtility.CreateButtonAction(name, wantsInitialStateCheck);
			this.m_InputActionValue = InputActionUtility.CreateValueAction(typeof(float), valueName ?? ((name != null) ? (name + " Value") : null));
			this.m_InputSourceMode = inputSourceMode;
		}

		public void EnableDirectActionIfModeUsed()
		{
			if (this.m_InputSourceMode == XRInputButtonReader.InputSourceMode.InputAction)
			{
				this.m_InputActionPerformed.Enable();
				this.m_InputActionValue.Enable();
			}
		}

		public void DisableDirectActionIfModeUsed()
		{
			if (this.m_InputSourceMode == XRInputButtonReader.InputSourceMode.InputAction)
			{
				this.m_InputActionPerformed.Disable();
				this.m_InputActionValue.Disable();
			}
		}

		public IXRInputButtonReader GetObjectReference()
		{
			return this.m_ObjectReference.Get(this.m_ObjectReferenceObject);
		}

		public void SetObjectReference(IXRInputButtonReader value)
		{
			this.m_ObjectReference.Set(ref this.m_ObjectReferenceObject, value);
		}

		public void QueueManualState(bool performed, float value)
		{
			this.QueueManualState(performed, value, !this.m_ManualPerformed && performed, this.m_ManualPerformed && !performed);
		}

		public void QueueManualState(bool performed, float value, bool performedThisFrame, bool completedThisFrame)
		{
			if (this.m_InputSourceMode != XRInputButtonReader.InputSourceMode.ManualValue)
			{
				Debug.LogWarning(string.Format("QueueManualState was called but the input source mode is set to {0}.", this.m_InputSourceMode) + "You may want to set inputSourceMode to ManualValue for the manual state to be effective next frame.");
			}
			this.m_ManualQueuePerformed = performed;
			this.m_ManualQueueWasPerformedThisFrame = performedThisFrame;
			this.m_ManualQueueWasCompletedThisFrame = completedThisFrame;
			this.m_ManualQueueValue = value;
			this.m_ManualQueueTargetFrame = Time.frameCount + 1;
		}

		private void RefreshManualIfNeeded()
		{
			if (this.m_ManualQueueTargetFrame > 0 && Time.frameCount >= this.m_ManualQueueTargetFrame)
			{
				this.m_ManualPerformed = this.m_ManualQueuePerformed;
				if (this.m_ManualQueueWasPerformedThisFrame)
				{
					this.m_ManualFramePerformed = Time.frameCount;
				}
				if (this.m_ManualQueueWasCompletedThisFrame)
				{
					this.m_ManualFrameCompleted = Time.frameCount;
				}
				this.m_ManualValue = this.m_ManualQueueValue;
				this.m_ManualQueueTargetFrame = 0;
			}
		}

		public bool ReadIsPerformed()
		{
			if (this.bypass != null && !this.m_CallingBypass)
			{
				using (new XRInputButtonReader.BypassScope(this))
				{
					return this.bypass.ReadIsPerformed();
				}
			}
			switch (this.m_InputSourceMode)
			{
			default:
				return false;
			case XRInputButtonReader.InputSourceMode.InputAction:
				return XRInputButtonReader.IsPerformed(this.m_InputActionPerformed);
			case XRInputButtonReader.InputSourceMode.InputActionReference:
			{
				InputActionReference inputActionReference;
				return this.TryGetInputActionReferencePerformed(out inputActionReference) && XRInputButtonReader.IsPerformed(inputActionReference.action);
			}
			case XRInputButtonReader.InputSourceMode.ObjectReference:
			{
				IXRInputButtonReader objectReference = this.GetObjectReference();
				return objectReference != null && objectReference.ReadIsPerformed();
			}
			case XRInputButtonReader.InputSourceMode.ManualValue:
				this.RefreshManualIfNeeded();
				return this.m_ManualPerformed;
			}
		}

		public bool ReadWasPerformedThisFrame()
		{
			if (this.bypass != null && !this.m_CallingBypass)
			{
				using (new XRInputButtonReader.BypassScope(this))
				{
					return this.bypass.ReadWasPerformedThisFrame();
				}
			}
			switch (this.m_InputSourceMode)
			{
			default:
				return false;
			case XRInputButtonReader.InputSourceMode.InputAction:
				return XRInputButtonReader.WasPerformedThisFrame(this.m_InputActionPerformed);
			case XRInputButtonReader.InputSourceMode.InputActionReference:
			{
				InputActionReference inputActionReference;
				return this.TryGetInputActionReferencePerformed(out inputActionReference) && XRInputButtonReader.WasPerformedThisFrame(inputActionReference.action);
			}
			case XRInputButtonReader.InputSourceMode.ObjectReference:
			{
				IXRInputButtonReader objectReference = this.GetObjectReference();
				return objectReference != null && objectReference.ReadWasPerformedThisFrame();
			}
			case XRInputButtonReader.InputSourceMode.ManualValue:
				this.RefreshManualIfNeeded();
				return this.m_ManualPerformed && this.m_ManualFramePerformed == Time.frameCount;
			}
		}

		public bool ReadWasCompletedThisFrame()
		{
			if (this.bypass != null && !this.m_CallingBypass)
			{
				using (new XRInputButtonReader.BypassScope(this))
				{
					return this.bypass.ReadWasCompletedThisFrame();
				}
			}
			switch (this.m_InputSourceMode)
			{
			default:
				return false;
			case XRInputButtonReader.InputSourceMode.InputAction:
				return XRInputButtonReader.WasCompletedThisFrame(this.m_InputActionPerformed);
			case XRInputButtonReader.InputSourceMode.InputActionReference:
			{
				InputActionReference inputActionReference;
				return this.TryGetInputActionReferencePerformed(out inputActionReference) && XRInputButtonReader.WasCompletedThisFrame(inputActionReference.action);
			}
			case XRInputButtonReader.InputSourceMode.ObjectReference:
			{
				IXRInputButtonReader objectReference = this.GetObjectReference();
				return objectReference != null && objectReference.ReadWasCompletedThisFrame();
			}
			case XRInputButtonReader.InputSourceMode.ManualValue:
				this.RefreshManualIfNeeded();
				return !this.m_ManualPerformed && this.m_ManualFrameCompleted == Time.frameCount;
			}
		}

		public float ReadValue()
		{
			if (this.bypass != null && !this.m_CallingBypass)
			{
				using (new XRInputButtonReader.BypassScope(this))
				{
					return this.bypass.ReadValue();
				}
			}
			switch (this.m_InputSourceMode)
			{
			default:
				return 0f;
			case XRInputButtonReader.InputSourceMode.InputAction:
				return this.ReadValueToFloat(this.m_InputActionValue);
			case XRInputButtonReader.InputSourceMode.InputActionReference:
			{
				InputActionReference inputActionReference;
				if (!this.TryGetInputActionReferenceValue(out inputActionReference))
				{
					return 0f;
				}
				return this.ReadValueToFloat(inputActionReference.action);
			}
			case XRInputButtonReader.InputSourceMode.ObjectReference:
			{
				IXRInputButtonReader objectReference = this.GetObjectReference();
				if (objectReference == null)
				{
					return 0f;
				}
				return objectReference.ReadValue();
			}
			case XRInputButtonReader.InputSourceMode.ManualValue:
				this.RefreshManualIfNeeded();
				return this.m_ManualValue;
			}
		}

		public bool TryReadValue(out float value)
		{
			if (this.bypass != null && !this.m_CallingBypass)
			{
				using (new XRInputButtonReader.BypassScope(this))
				{
					return this.bypass.TryReadValue(out value);
				}
			}
			switch (this.m_InputSourceMode)
			{
			default:
				value = 0f;
				return false;
			case XRInputButtonReader.InputSourceMode.InputAction:
				return this.TryReadValue(this.m_InputActionValue, out value);
			case XRInputButtonReader.InputSourceMode.InputActionReference:
			{
				InputActionReference inputActionReference;
				if (this.TryGetInputActionReferenceValue(out inputActionReference))
				{
					return this.TryReadValue(inputActionReference.action, out value);
				}
				value = 0f;
				return false;
			}
			case XRInputButtonReader.InputSourceMode.ObjectReference:
			{
				IXRInputButtonReader objectReference = this.GetObjectReference();
				if (objectReference != null)
				{
					return objectReference.TryReadValue(out value);
				}
				value = 0f;
				return false;
			}
			case XRInputButtonReader.InputSourceMode.ManualValue:
				this.RefreshManualIfNeeded();
				value = this.m_ManualValue;
				return true;
			}
		}

		private static bool IsPerformed(InputAction action)
		{
			if (action == null)
			{
				return false;
			}
			InputActionPhase phase = action.phase;
			return phase == InputActionPhase.Performed || (phase != InputActionPhase.Disabled && action.WasPerformedThisFrame());
		}

		private static bool WasPerformedThisFrame(InputAction action)
		{
			return action != null && action.WasPerformedThisFrame();
		}

		private static bool WasCompletedThisFrame(InputAction action)
		{
			return action != null && action.WasCompletedThisFrame();
		}

		private float ReadValueToFloat(InputAction action)
		{
			if (action == null)
			{
				return 0f;
			}
			Type activeValueType = action.activeValueType;
			if (activeValueType == null || activeValueType == typeof(float))
			{
				return action.ReadValue<float>();
			}
			if (activeValueType == typeof(Vector2))
			{
				return action.ReadValue<Vector2>().magnitude;
			}
			return Mathf.Max(action.GetControlMagnitude(), 0f);
		}

		private bool TryReadValue(InputAction action, out float value)
		{
			if (action == null)
			{
				value = 0f;
				return false;
			}
			value = this.ReadValueToFloat(action);
			return action.IsInProgress();
		}

		private bool TryGetInputActionReferencePerformed(out InputActionReference reference)
		{
			return this.m_InputActionReferencePerformedCache.TryGet(this.m_InputActionReferencePerformed, out reference);
		}

		private bool TryGetInputActionReferenceValue(out InputActionReference reference)
		{
			return this.m_InputActionReferenceValueCache.TryGet(this.m_InputActionReferenceValue, out reference);
		}

		[SerializeField]
		private XRInputButtonReader.InputSourceMode m_InputSourceMode = XRInputButtonReader.InputSourceMode.InputActionReference;

		[SerializeField]
		private InputAction m_InputActionPerformed;

		[SerializeField]
		private InputAction m_InputActionValue;

		[SerializeField]
		private InputActionReference m_InputActionReferencePerformed;

		[SerializeField]
		private InputActionReference m_InputActionReferenceValue;

		[SerializeField]
		[RequireInterface(typeof(IXRInputButtonReader))]
		private Object m_ObjectReferenceObject;

		[SerializeField]
		private bool m_ManualPerformed;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_ManualValue;

		[SerializeField]
		private bool m_ManualQueuePerformed;

		[SerializeField]
		private bool m_ManualQueueWasPerformedThisFrame;

		[SerializeField]
		private bool m_ManualQueueWasCompletedThisFrame;

		[SerializeField]
		private float m_ManualQueueValue;

		[SerializeField]
		private int m_ManualQueueTargetFrame;

		private int m_ManualFramePerformed;

		private int m_ManualFrameCompleted;

		private bool m_CallingBypass;

		private readonly UnityObjectReferenceCache<IXRInputButtonReader, Object> m_ObjectReference = new UnityObjectReferenceCache<IXRInputButtonReader, Object>();

		private readonly UnityObjectReferenceCache<InputActionReference> m_InputActionReferencePerformedCache = new UnityObjectReferenceCache<InputActionReference>();

		private readonly UnityObjectReferenceCache<InputActionReference> m_InputActionReferenceValueCache = new UnityObjectReferenceCache<InputActionReference>();

		private struct BypassScope : IDisposable
		{
			public BypassScope(XRInputButtonReader reader)
			{
				this.m_Reader = reader;
				this.m_Reader.m_CallingBypass = true;
			}

			public void Dispose()
			{
				this.m_Reader.m_CallingBypass = false;
			}

			private readonly XRInputButtonReader m_Reader;
		}

		public enum InputSourceMode
		{
			Unused,
			InputAction,
			InputActionReference,
			ObjectReference,
			ManualValue
		}
	}
}
