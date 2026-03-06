using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement
{
	[AddComponentMenu("XR/Locomotion/Grab Move Provider", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement.GrabMoveProvider.html")]
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	public class GrabMoveProvider : ConstrainedMoveProvider
	{
		public Transform controllerTransform
		{
			get
			{
				return this.m_ControllerTransform;
			}
			set
			{
				this.m_ControllerTransform = value;
				this.GatherControllerInteractors();
			}
		}

		public bool enableMoveWhileSelecting
		{
			get
			{
				return this.m_EnableMoveWhileSelecting;
			}
			set
			{
				this.m_EnableMoveWhileSelecting = value;
			}
		}

		public float moveFactor
		{
			get
			{
				return this.m_MoveFactor;
			}
			set
			{
				this.m_MoveFactor = value;
			}
		}

		public XRInputButtonReader grabMoveInput
		{
			get
			{
				return this.m_GrabMoveInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_GrabMoveInput, value, this);
			}
		}

		public bool canMove { get; set; } = true;

		protected override void Awake()
		{
			base.Awake();
			if (this.m_ControllerTransform == null)
			{
				this.m_ControllerTransform = base.transform;
			}
			this.GatherControllerInteractors();
			if (this.m_GrabMoveAction.reference != null || (this.m_GrabMoveAction.action != null && this.m_GrabMoveAction.action.bindings.Count > 0))
			{
				Debug.LogWarning("Grab Move Action has been deprecated. Please configure input action using Grab Move Input instead.", this);
			}
		}

		protected void OnEnable()
		{
			this.m_GrabMoveInput.EnableDirectActionIfModeUsed();
			this.m_GrabMoveAction.EnableDirectAction();
		}

		protected void OnDisable()
		{
			this.m_GrabMoveInput.DisableDirectActionIfModeUsed();
			this.m_GrabMoveAction.DisableDirectAction();
		}

		protected override Vector3 ComputeDesiredMove(out bool attemptingMove)
		{
			attemptingMove = false;
			XROrigin xrOrigin = base.mediator.xrOrigin;
			GameObject gameObject = (xrOrigin != null) ? xrOrigin.Origin : null;
			bool isMoving = this.m_IsMoving;
			this.m_IsMoving = (this.canMove && this.IsGrabbing() && gameObject != null);
			if (!this.m_IsMoving)
			{
				return Vector3.zero;
			}
			Vector3 localPosition = this.controllerTransform.localPosition;
			if (!isMoving && this.m_IsMoving)
			{
				this.m_PreviousControllerLocalPosition = localPosition;
				return Vector3.zero;
			}
			attemptingMove = true;
			Vector3 result = gameObject.transform.TransformVector(this.m_PreviousControllerLocalPosition - localPosition) * this.m_MoveFactor;
			this.m_PreviousControllerLocalPosition = localPosition;
			return result;
		}

		public bool IsGrabbing()
		{
			bool flag = this.m_GrabMoveInput.ReadIsPerformed();
			InputAction action = this.m_GrabMoveAction.action;
			if (action != null)
			{
				flag |= action.IsPressed();
			}
			return flag && (this.m_EnableMoveWhileSelecting || !this.ControllerHasSelection());
		}

		private void GatherControllerInteractors()
		{
			this.m_ControllerInteractors.Clear();
			if (this.m_ControllerTransform != null)
			{
				this.m_ControllerTransform.transform.GetComponentsInChildren<IXRSelectInteractor>(this.m_ControllerInteractors);
			}
		}

		private bool ControllerHasSelection()
		{
			for (int i = 0; i < this.m_ControllerInteractors.Count; i++)
			{
				if (this.m_ControllerInteractors[i].hasSelection)
				{
					return true;
				}
			}
			return false;
		}

		[Obsolete("grabMoveAction has been deprecated. Please configure input action using grabMoveInput instead.")]
		public InputActionProperty grabMoveAction
		{
			get
			{
				return this.m_GrabMoveAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_GrabMoveAction, value);
			}
		}

		private void SetInputActionProperty(ref InputActionProperty property, InputActionProperty value)
		{
			if (Application.isPlaying)
			{
				property.DisableDirectAction();
			}
			property = value;
			if (Application.isPlaying && base.isActiveAndEnabled)
			{
				property.EnableDirectAction();
			}
		}

		[SerializeField]
		private Transform m_ControllerTransform;

		[SerializeField]
		private bool m_EnableMoveWhileSelecting;

		[SerializeField]
		private float m_MoveFactor = 1f;

		[SerializeField]
		private XRInputButtonReader m_GrabMoveInput = new XRInputButtonReader("Grab Move", null, false, XRInputButtonReader.InputSourceMode.InputActionReference);

		private bool m_IsMoving;

		private Vector3 m_PreviousControllerLocalPosition;

		private readonly List<IXRSelectInteractor> m_ControllerInteractors = new List<IXRSelectInteractor>();

		[SerializeField]
		[Obsolete("m_GrabMoveAction has been deprecated. Please configure input action using m_GrabMoveInput instead.")]
		private InputActionProperty m_GrabMoveAction = new InputActionProperty(new InputAction("Grab Move", InputActionType.Button, null, null, null, null));
	}
}
