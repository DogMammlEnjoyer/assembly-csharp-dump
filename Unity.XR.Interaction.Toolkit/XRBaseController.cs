using System;
using Unity.XR.CoreUtils;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[DefaultExecutionOrder(-29990)]
	[DisallowMultipleComponent]
	[Obsolete("XRBaseController has been deprecated in version 3.0.0. Its functionality has been distributed into different components.")]
	public abstract class XRBaseController : MonoBehaviour, IXRHapticImpulseProvider
	{
		[Obsolete("GetControllerState has been deprecated. Use currentControllerState instead.", true)]
		public virtual bool GetControllerState(out XRControllerState controllerState)
		{
			controllerState = null;
			return false;
		}

		[Obsolete("SetControllerState has been deprecated. Use currentControllerState instead.", true)]
		public virtual void SetControllerState(XRControllerState controllerState)
		{
		}

		[Obsolete("modelTransform has been deprecated due to being renamed. Use modelParent instead. (UnityUpgradable) -> modelParent", true)]
		public Transform modelTransform
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("anchorControlDeadzone is obsolete. Please configure deadzone on the Rotate Anchor and Translate Anchor Actions.", true)]
		public float anchorControlDeadzone
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[Obsolete("anchorControlOffAxisDeadzone is obsolete. Please configure deadzone on the Rotate Anchor and Translate Anchor Actions.", true)]
		public float anchorControlOffAxisDeadzone
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		public XRBaseController.UpdateType updateTrackingType
		{
			get
			{
				return this.m_UpdateTrackingType;
			}
			set
			{
				this.m_UpdateTrackingType = value;
			}
		}

		public bool enableInputTracking
		{
			get
			{
				return this.m_EnableInputTracking;
			}
			set
			{
				this.m_EnableInputTracking = value;
			}
		}

		public bool enableInputActions
		{
			get
			{
				return this.m_EnableInputActions;
			}
			set
			{
				this.m_EnableInputActions = value;
			}
		}

		public Transform modelPrefab
		{
			get
			{
				return this.m_ModelPrefab;
			}
			set
			{
				this.m_ModelPrefab = value;
			}
		}

		public Transform modelParent
		{
			get
			{
				return this.m_ModelParent;
			}
			set
			{
				this.m_ModelParent = value;
				if (this.m_Model != null)
				{
					this.m_Model.parent = this.m_ModelParent;
				}
			}
		}

		public Transform model
		{
			get
			{
				return this.m_Model;
			}
			set
			{
				this.m_Model = value;
			}
		}

		public bool animateModel
		{
			get
			{
				return this.m_AnimateModel;
			}
			set
			{
				this.m_AnimateModel = value;
			}
		}

		public string modelSelectTransition
		{
			get
			{
				return this.m_ModelSelectTransition;
			}
			set
			{
				this.m_ModelSelectTransition = value;
			}
		}

		public string modelDeSelectTransition
		{
			get
			{
				return this.m_ModelDeSelectTransition;
			}
			set
			{
				this.m_ModelDeSelectTransition = value;
			}
		}

		public bool hideControllerModel
		{
			get
			{
				return this.m_HideControllerModel;
			}
			set
			{
				this.m_HideControllerModel = value;
				if (this.m_Model != null)
				{
					this.m_Model.gameObject.SetActive(!this.m_HideControllerModel);
				}
			}
		}

		public InteractionState selectInteractionState
		{
			get
			{
				return this.m_SelectInteractionState;
			}
		}

		public InteractionState activateInteractionState
		{
			get
			{
				return this.m_ActivateInteractionState;
			}
		}

		public InteractionState uiPressInteractionState
		{
			get
			{
				return this.m_UIPressInteractionState;
			}
		}

		public Vector2 uiScrollValue
		{
			get
			{
				return this.m_UIScrollValue;
			}
		}

		public XRControllerState currentControllerState
		{
			get
			{
				this.SetupControllerState();
				return this.m_ControllerState;
			}
			set
			{
				this.m_ControllerState = value;
				this.m_CreateControllerState = false;
			}
		}

		protected virtual void Awake()
		{
			if (this.m_ModelParent == null)
			{
				this.m_ModelParent = new GameObject("[" + base.gameObject.name + "] Model Parent").transform;
				this.m_ModelParent.SetParent(base.transform, false);
				this.m_ModelParent.SetLocalPose(Pose.identity);
			}
		}

		protected virtual void OnEnable()
		{
			Application.onBeforeRender += this.OnBeforeRender;
		}

		protected virtual void OnDisable()
		{
			Application.onBeforeRender -= this.OnBeforeRender;
		}

		protected void Update()
		{
			this.UpdateController();
		}

		private void SetupModel()
		{
			if (this.m_Model == null)
			{
				GameObject modelPrefab = this.GetModelPrefab();
				if (modelPrefab != null)
				{
					this.m_Model = Object.Instantiate<GameObject>(modelPrefab, this.m_ModelParent).transform;
				}
			}
			if (this.m_Model != null)
			{
				this.m_Model.gameObject.SetActive(!this.m_HideControllerModel);
			}
		}

		private void SetupControllerState()
		{
			if (this.m_ControllerState == null && this.m_CreateControllerState)
			{
				this.m_ControllerState = new XRControllerState();
			}
		}

		protected virtual GameObject GetModelPrefab()
		{
			if (!(this.m_ModelPrefab != null))
			{
				return null;
			}
			return this.m_ModelPrefab.gameObject;
		}

		protected virtual void UpdateController()
		{
			if (this.m_PerformSetup)
			{
				this.SetupModel();
				this.SetupControllerState();
				this.m_PerformSetup = false;
			}
			if (this.m_EnableInputTracking && (this.m_UpdateTrackingType == XRBaseController.UpdateType.Update || this.m_UpdateTrackingType == XRBaseController.UpdateType.UpdateAndBeforeRender))
			{
				this.UpdateTrackingInput(this.m_ControllerState);
			}
			if (this.m_EnableInputActions)
			{
				this.UpdateInput(this.m_ControllerState);
				this.UpdateControllerModelAnimation();
			}
			this.ApplyControllerState(XRInteractionUpdateOrder.UpdatePhase.Dynamic, this.m_ControllerState);
		}

		protected virtual void OnBeforeRender()
		{
			if (this.m_EnableInputTracking && (this.m_UpdateTrackingType == XRBaseController.UpdateType.BeforeRender || this.m_UpdateTrackingType == XRBaseController.UpdateType.UpdateAndBeforeRender))
			{
				this.UpdateTrackingInput(this.m_ControllerState);
			}
			this.ApplyControllerState(XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender, this.m_ControllerState);
		}

		protected virtual void FixedUpdate()
		{
			if (this.m_EnableInputTracking && this.m_UpdateTrackingType == XRBaseController.UpdateType.Fixed)
			{
				this.UpdateTrackingInput(this.m_ControllerState);
			}
			this.ApplyControllerState(XRInteractionUpdateOrder.UpdatePhase.Fixed, this.m_ControllerState);
		}

		protected virtual void ApplyControllerState(XRInteractionUpdateOrder.UpdatePhase updatePhase, XRControllerState controllerState)
		{
			if (controllerState == null)
			{
				return;
			}
			if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
			{
				this.m_SelectInteractionState = controllerState.selectInteractionState;
				this.m_ActivateInteractionState = controllerState.activateInteractionState;
				this.m_UIPressInteractionState = controllerState.uiPressInteractionState;
				this.m_UIScrollValue = controllerState.uiScrollValue;
			}
			if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic || updatePhase == XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender || updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
			{
				bool flag = (controllerState.inputTrackingState & InputTrackingState.Position) > InputTrackingState.None;
				bool flag2 = (controllerState.inputTrackingState & InputTrackingState.Rotation) > InputTrackingState.None;
				if (flag && flag2)
				{
					base.transform.SetLocalPose(new Pose(controllerState.position, controllerState.rotation));
					return;
				}
				if (flag)
				{
					base.transform.localPosition = controllerState.position;
					return;
				}
				if (flag2)
				{
					base.transform.localRotation = controllerState.rotation;
				}
			}
		}

		protected virtual void UpdateTrackingInput(XRControllerState controllerState)
		{
		}

		protected virtual void UpdateInput(XRControllerState controllerState)
		{
		}

		protected virtual void UpdateControllerModelAnimation()
		{
			if (this.m_AnimateModel && this.m_Model != null)
			{
				if ((this.m_ModelAnimator == null || this.m_ModelAnimator.gameObject != this.m_Model.gameObject) && !this.m_Model.TryGetComponent<Animator>(out this.m_ModelAnimator))
				{
					if (!this.m_HasWarnedAnimatorMissing)
					{
						Debug.LogWarning("Animate Model is enabled, but there is no Animator component on the model. Unable to activate named triggers to animate the model.", this);
						this.m_HasWarnedAnimatorMissing = true;
					}
					return;
				}
				if (this.m_SelectInteractionState.activatedThisFrame)
				{
					this.m_ModelAnimator.SetTrigger(this.m_ModelSelectTransition);
					return;
				}
				if (this.m_SelectInteractionState.deactivatedThisFrame)
				{
					this.m_ModelAnimator.SetTrigger(this.m_ModelDeSelectTransition);
				}
			}
		}

		public virtual bool SendHapticImpulse(float amplitude, float duration)
		{
			return false;
		}

		IXRHapticImpulseChannelGroup IXRHapticImpulseProvider.GetChannelGroup()
		{
			if (this.m_HapticChannel == null)
			{
				this.m_HapticChannel = new XRBaseController.HapticImpulseChannel(this);
			}
			HapticImpulseSingleChannelGroup result;
			if ((result = this.m_HapticChannelGroup) == null)
			{
				result = (this.m_HapticChannelGroup = new HapticImpulseSingleChannelGroup(this.m_HapticChannel));
			}
			return result;
		}

		[SerializeField]
		private XRBaseController.UpdateType m_UpdateTrackingType;

		[SerializeField]
		private bool m_EnableInputTracking = true;

		[SerializeField]
		private bool m_EnableInputActions = true;

		[SerializeField]
		private Transform m_ModelPrefab;

		[SerializeField]
		[FormerlySerializedAs("m_ModelTransform")]
		private Transform m_ModelParent;

		[SerializeField]
		private Transform m_Model;

		[SerializeField]
		private bool m_AnimateModel;

		[SerializeField]
		private string m_ModelSelectTransition;

		[SerializeField]
		private string m_ModelDeSelectTransition;

		private bool m_HideControllerModel;

		private InteractionState m_SelectInteractionState;

		private InteractionState m_ActivateInteractionState;

		private InteractionState m_UIPressInteractionState;

		private Vector2 m_UIScrollValue;

		private XRControllerState m_ControllerState;

		private bool m_CreateControllerState = true;

		private Animator m_ModelAnimator;

		private bool m_HasWarnedAnimatorMissing;

		private bool m_PerformSetup = true;

		private XRBaseController.HapticImpulseChannel m_HapticChannel;

		private HapticImpulseSingleChannelGroup m_HapticChannelGroup;

		public enum UpdateType
		{
			UpdateAndBeforeRender,
			Update,
			BeforeRender,
			Fixed
		}

		private class HapticImpulseChannel : IXRHapticImpulseChannel
		{
			public HapticImpulseChannel(XRBaseController controller)
			{
				this.m_Controller = controller;
			}

			public bool SendHapticImpulse(float amplitude, float duration, float frequency)
			{
				if (frequency > 0f && !this.m_WarningLogged)
				{
					Debug.LogWarning(string.Format("Frequency is not supported when using {0} as the haptic impulse channel.", this.m_Controller) + " You may need to update the HapticImpulsePlayer to use an Input Action Reference with a Haptic control binding rather than using an Object Reference to the controller.", this.m_Controller);
					this.m_WarningLogged = true;
				}
				return this.m_Controller.SendHapticImpulse(amplitude, duration);
			}

			private readonly XRBaseController m_Controller;

			private bool m_WarningLogged;
		}
	}
}
