using System;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Readers
{
	public abstract class XRInputValueReader
	{
		public XRInputValueReader.InputSourceMode inputSourceMode
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

		public InputAction inputAction
		{
			get
			{
				return this.m_InputAction;
			}
			set
			{
				this.m_InputAction = value;
			}
		}

		public InputActionReference inputActionReference
		{
			get
			{
				return this.m_InputActionReference;
			}
			set
			{
				this.m_InputActionReference = value;
			}
		}

		protected XRInputValueReader()
		{
		}

		protected XRInputValueReader(InputAction inputAction, XRInputValueReader.InputSourceMode inputSourceMode)
		{
			this.m_InputAction = inputAction;
			this.m_InputSourceMode = inputSourceMode;
		}

		public void EnableDirectActionIfModeUsed()
		{
			if (this.m_InputSourceMode == XRInputValueReader.InputSourceMode.InputAction)
			{
				this.m_InputAction.Enable();
			}
		}

		public void DisableDirectActionIfModeUsed()
		{
			if (this.m_InputSourceMode == XRInputValueReader.InputSourceMode.InputAction)
			{
				this.m_InputAction.Disable();
			}
		}

		private protected bool TryGetInputActionReference(out InputActionReference reference)
		{
			return this.m_InputActionReferenceCache.TryGet(this.m_InputActionReference, out reference);
		}

		[SerializeField]
		private protected XRInputValueReader.InputSourceMode m_InputSourceMode = XRInputValueReader.InputSourceMode.InputActionReference;

		[SerializeField]
		private protected InputAction m_InputAction;

		[SerializeField]
		private InputActionReference m_InputActionReference;

		private readonly UnityObjectReferenceCache<InputActionReference> m_InputActionReferenceCache = new UnityObjectReferenceCache<InputActionReference>();

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
