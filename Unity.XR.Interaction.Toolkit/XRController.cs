using System;
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.SpatialTracking;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[AddComponentMenu("/XR Controller (Device-based)", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.XRController.html")]
	[Obsolete("XRController has been deprecated in version 3.0.0. Its functionality has been distributed into different components.")]
	public class XRController : XRBaseController
	{
		public XRNode controllerNode
		{
			get
			{
				return this.m_ControllerNode;
			}
			set
			{
				this.m_ControllerNode = value;
			}
		}

		public InputHelpers.Button selectUsage
		{
			get
			{
				return this.m_SelectUsage;
			}
			set
			{
				this.m_SelectUsage = value;
			}
		}

		public InputHelpers.Button activateUsage
		{
			get
			{
				return this.m_ActivateUsage;
			}
			set
			{
				this.m_ActivateUsage = value;
			}
		}

		public InputHelpers.Button uiPressUsage
		{
			get
			{
				return this.m_UIPressUsage;
			}
			set
			{
				this.m_UIPressUsage = value;
			}
		}

		public float axisToPressThreshold
		{
			get
			{
				return this.m_AxisToPressThreshold;
			}
			set
			{
				this.m_AxisToPressThreshold = value;
			}
		}

		public InputHelpers.Button rotateObjectLeft
		{
			get
			{
				return this.m_RotateAnchorLeft;
			}
			set
			{
				this.m_RotateAnchorLeft = value;
			}
		}

		public InputHelpers.Button rotateObjectRight
		{
			get
			{
				return this.m_RotateAnchorRight;
			}
			set
			{
				this.m_RotateAnchorRight = value;
			}
		}

		public InputHelpers.Button moveObjectIn
		{
			get
			{
				return this.m_MoveObjectIn;
			}
			set
			{
				this.m_MoveObjectIn = value;
			}
		}

		public InputHelpers.Button moveObjectOut
		{
			get
			{
				return this.m_MoveObjectOut;
			}
			set
			{
				this.m_MoveObjectOut = value;
			}
		}

		public InputHelpers.Axis2D directionalAnchorRotation
		{
			get
			{
				return this.m_DirectionalAnchorRotation;
			}
			set
			{
				this.m_DirectionalAnchorRotation = value;
			}
		}

		public BasePoseProvider poseProvider
		{
			get
			{
				return this.m_PoseProvider;
			}
			set
			{
				this.m_PoseProvider = value;
			}
		}

		public InputDevice inputDevice
		{
			get
			{
				if (this.m_InputDeviceControllerNode != this.m_ControllerNode || !this.m_InputDevice.isValid)
				{
					this.m_InputDevice = InputDevices.GetDeviceAtXRNode(this.m_ControllerNode);
					this.m_InputDeviceControllerNode = this.m_ControllerNode;
				}
				return this.m_InputDevice;
			}
		}

		protected override void Awake()
		{
			base.Awake();
		}

		protected override void UpdateTrackingInput(XRControllerState controllerState)
		{
			base.UpdateTrackingInput(controllerState);
			if (controllerState == null)
			{
				return;
			}
			bool flag;
			controllerState.isTracked = (this.inputDevice.TryGetFeatureValue(CommonUsages.isTracked, out flag) && flag);
			controllerState.inputTrackingState = InputTrackingState.None;
			InputTrackingState inputTrackingState;
			if (this.m_PoseProvider != null)
			{
				Pose pose;
				PoseDataFlags poseFromProvider = this.m_PoseProvider.GetPoseFromProvider(out pose);
				if ((poseFromProvider & PoseDataFlags.Position) != PoseDataFlags.NoData)
				{
					controllerState.position = pose.position;
					controllerState.inputTrackingState |= InputTrackingState.Position;
				}
				if ((poseFromProvider & PoseDataFlags.Rotation) != PoseDataFlags.NoData)
				{
					controllerState.rotation = pose.rotation;
					controllerState.inputTrackingState |= InputTrackingState.Rotation;
					return;
				}
			}
			else if (this.inputDevice.TryGetFeatureValue(CommonUsages.trackingState, out inputTrackingState))
			{
				controllerState.inputTrackingState = inputTrackingState;
				Vector3 position;
				if ((inputTrackingState & InputTrackingState.Position) != InputTrackingState.None && this.inputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out position))
				{
					controllerState.position = position;
				}
				Quaternion rotation;
				if ((inputTrackingState & InputTrackingState.Rotation) != InputTrackingState.None && this.inputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation))
				{
					controllerState.rotation = rotation;
				}
			}
		}

		protected override void UpdateInput(XRControllerState controllerState)
		{
			base.UpdateInput(controllerState);
			if (controllerState == null)
			{
				return;
			}
			controllerState.ResetFrameDependentStates();
			controllerState.selectInteractionState.SetFrameState(this.IsPressed(this.m_SelectUsage), this.ReadValue(this.m_SelectUsage));
			controllerState.activateInteractionState.SetFrameState(this.IsPressed(this.m_ActivateUsage), this.ReadValue(this.m_ActivateUsage));
			controllerState.uiPressInteractionState.SetFrameState(this.IsPressed(this.m_UIPressUsage), this.ReadValue(this.m_UIPressUsage));
		}

		protected virtual bool IsPressed(InputHelpers.Button button)
		{
			bool result;
			this.inputDevice.IsPressed(button, out result, this.m_AxisToPressThreshold);
			return result;
		}

		protected virtual float ReadValue(InputHelpers.Button button)
		{
			float result;
			this.inputDevice.TryReadSingleValue(button, out result);
			return result;
		}

		public override bool SendHapticImpulse(float amplitude, float duration)
		{
			HapticCapabilities hapticCapabilities;
			return this.inputDevice.TryGetHapticCapabilities(out hapticCapabilities) && hapticCapabilities.supportsImpulse && this.inputDevice.SendHapticImpulse(0U, amplitude, duration);
		}

		[SerializeField]
		private XRNode m_ControllerNode = XRNode.RightHand;

		private XRNode m_InputDeviceControllerNode;

		[SerializeField]
		private InputHelpers.Button m_SelectUsage = InputHelpers.Button.Grip;

		[SerializeField]
		private InputHelpers.Button m_ActivateUsage = InputHelpers.Button.Trigger;

		[SerializeField]
		private InputHelpers.Button m_UIPressUsage = InputHelpers.Button.Trigger;

		[SerializeField]
		private float m_AxisToPressThreshold = 0.1f;

		[SerializeField]
		private InputHelpers.Button m_RotateAnchorLeft = InputHelpers.Button.PrimaryAxis2DLeft;

		[SerializeField]
		private InputHelpers.Button m_RotateAnchorRight = InputHelpers.Button.PrimaryAxis2DRight;

		[SerializeField]
		private InputHelpers.Button m_MoveObjectIn = InputHelpers.Button.PrimaryAxis2DUp;

		[SerializeField]
		private InputHelpers.Button m_MoveObjectOut = InputHelpers.Button.PrimaryAxis2DDown;

		[SerializeField]
		private InputHelpers.Axis2D m_DirectionalAnchorRotation = InputHelpers.Axis2D.PrimaryAxis2D;

		[SerializeField]
		private BasePoseProvider m_PoseProvider;

		private InputDevice m_InputDevice;
	}
}
