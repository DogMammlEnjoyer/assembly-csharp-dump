using System;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
	[Serializable]
	public class XRInputHapticImpulseProvider : IXRHapticImpulseProvider
	{
		public XRInputHapticImpulseProvider.InputSourceMode inputSourceMode
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

		public XRInputHapticImpulseProvider()
		{
		}

		public XRInputHapticImpulseProvider(string name = null, bool wantsInitialStateCheck = false, XRInputHapticImpulseProvider.InputSourceMode inputSourceMode = XRInputHapticImpulseProvider.InputSourceMode.InputActionReference)
		{
			this.m_InputAction = InputActionUtility.CreatePassThroughAction(null, name, wantsInitialStateCheck);
			this.m_InputSourceMode = inputSourceMode;
		}

		public void EnableDirectActionIfModeUsed()
		{
			if (this.m_InputSourceMode == XRInputHapticImpulseProvider.InputSourceMode.InputAction)
			{
				this.m_InputAction.Enable();
			}
		}

		public void DisableDirectActionIfModeUsed()
		{
			if (this.m_InputSourceMode == XRInputHapticImpulseProvider.InputSourceMode.InputAction)
			{
				this.m_InputAction.Disable();
			}
		}

		public IXRHapticImpulseProvider GetObjectReference()
		{
			return this.m_ObjectReference.Get(this.m_ObjectReferenceObject);
		}

		public void SetObjectReference(IXRHapticImpulseProvider value)
		{
			this.m_ObjectReference.Set(ref this.m_ObjectReferenceObject, value);
		}

		public IXRHapticImpulseChannelGroup GetChannelGroup()
		{
			switch (this.m_InputSourceMode)
			{
			default:
				return null;
			case XRInputHapticImpulseProvider.InputSourceMode.InputAction:
				if (this.m_HapticControlActionManager == null)
				{
					this.m_HapticControlActionManager = new HapticControlActionManager();
				}
				return this.m_HapticControlActionManager.GetChannelGroup(this.m_InputAction);
			case XRInputHapticImpulseProvider.InputSourceMode.InputActionReference:
				if (this.m_InputActionReference != null)
				{
					if (this.m_HapticControlActionManager == null)
					{
						this.m_HapticControlActionManager = new HapticControlActionManager();
					}
					return this.m_HapticControlActionManager.GetChannelGroup(this.m_InputActionReference.action);
				}
				return null;
			case XRInputHapticImpulseProvider.InputSourceMode.ObjectReference:
			{
				IXRHapticImpulseProvider objectReference = this.GetObjectReference();
				if (objectReference == null)
				{
					return null;
				}
				return objectReference.GetChannelGroup();
			}
			}
		}

		[SerializeField]
		private XRInputHapticImpulseProvider.InputSourceMode m_InputSourceMode = XRInputHapticImpulseProvider.InputSourceMode.InputActionReference;

		[SerializeField]
		private InputAction m_InputAction;

		[SerializeField]
		private InputActionReference m_InputActionReference;

		[SerializeField]
		[RequireInterface(typeof(IXRHapticImpulseProvider))]
		private Object m_ObjectReferenceObject;

		private readonly UnityObjectReferenceCache<IXRHapticImpulseProvider, Object> m_ObjectReference = new UnityObjectReferenceCache<IXRHapticImpulseProvider, Object>();

		private HapticControlActionManager m_HapticControlActionManager;

		public enum InputSourceMode
		{
			Unused,
			InputAction,
			InputActionReference,
			ObjectReference
		}
	}
}
