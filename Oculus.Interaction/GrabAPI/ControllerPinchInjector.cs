using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.GrabAPI
{
	public class ControllerPinchInjector : MonoBehaviour
	{
		private IController Controller { get; set; }

		protected virtual void Awake()
		{
			this.Controller = (this._controller as IController);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this._handGrabAPI.InjectOptionalFingerPinchAPI(new ControllerPinchInjector.ControllerPinchAPI(this.Controller));
			this._handGrabAPI.InjectOptionalFingerGrabAPI(new ControllerPinchInjector.ControllerPinchAPI(this.Controller));
			this.EndStart(ref this._started);
		}

		public void InjectAll(HandGrabAPI handGrabAPI, IController controller)
		{
			this.InjectHandGrabAPI(handGrabAPI);
			this.InjectController(controller);
		}

		public void InjectHandGrabAPI(HandGrabAPI handGrabAPI)
		{
			this._handGrabAPI = handGrabAPI;
		}

		public void InjectController(IController controller)
		{
			this._controller = (controller as Object);
			this.Controller = controller;
		}

		[SerializeField]
		private HandGrabAPI _handGrabAPI;

		[SerializeField]
		[Interface(typeof(IController), new Type[]
		{

		})]
		private Object _controller;

		protected bool _started;

		private class ControllerPinchAPI : IFingerAPI
		{
			public ControllerPinchAPI(IController controller)
			{
				this._controller = controller;
			}

			public float GetFingerGrabScore(HandFinger finger)
			{
				switch (finger)
				{
				case HandFinger.Thumb:
					return this._triggerStrength;
				case HandFinger.Index:
					return this._triggerStrength;
				case HandFinger.Middle:
				case HandFinger.Ring:
				case HandFinger.Pinky:
					return this._gripStrength;
				default:
					return 0f;
				}
			}

			public bool GetFingerIsGrabbing(HandFinger finger)
			{
				switch (finger)
				{
				case HandFinger.Thumb:
					return this._triggerDown;
				case HandFinger.Index:
					return this._triggerDown;
				case HandFinger.Middle:
				case HandFinger.Ring:
				case HandFinger.Pinky:
					return this._gripDown;
				default:
					return false;
				}
			}

			public bool GetFingerIsGrabbingChanged(HandFinger finger, bool targetPinchState)
			{
				switch (finger)
				{
				case HandFinger.Thumb:
					return (this._triggerDown == targetPinchState && this._triggerDown != this._prevTriggerDown) || (this._gripDown == targetPinchState && this._gripDown != this._prevGripDown);
				case HandFinger.Index:
					return this._triggerDown == targetPinchState && this._triggerDown != this._prevTriggerDown;
				case HandFinger.Middle:
				case HandFinger.Ring:
				case HandFinger.Pinky:
					return this._gripDown == targetPinchState && this._gripDown != this._prevGripDown;
				default:
					return false;
				}
			}

			public Vector3 GetWristOffsetLocal()
			{
				return this._pinchPose.position;
			}

			public void Update(IHand hand)
			{
				ControllerInput controllerInput = this._controller.ControllerInput;
				this._prevGripDown = this._gripDown;
				this._prevTriggerDown = this._triggerDown;
				this._triggerStrength = controllerInput.Trigger;
				this._triggerDown = controllerInput.TriggerButton;
				this._gripStrength = controllerInput.Grip;
				this._gripDown = controllerInput.GripButton;
				hand.GetJointPoseFromWrist(HandJointId.HandIndexTip, out this._indexPinchPose);
				hand.GetJointPoseFromWrist(HandJointId.HandMiddleTip, out this._middlePinchPose);
				Pose pose;
				hand.GetJointPoseFromWrist(HandJointId.HandThumbTip, out pose);
				ref this._indexPinchPose.Lerp(pose, 0.5f);
				ref this._middlePinchPose.Lerp(pose, 0.5f);
				float num = this._triggerStrength + this._gripStrength;
				float t = (num > 0f) ? (this._gripStrength / num) : 0.5f;
				PoseUtils.Lerp(this._indexPinchPose, this._middlePinchPose, t, ref this._pinchPose);
			}

			private IController _controller;

			private float _triggerStrength;

			private float _gripStrength;

			private bool _triggerDown;

			private bool _gripDown;

			private bool _prevTriggerDown;

			private bool _prevGripDown;

			private Pose _indexPinchPose = Pose.identity;

			private Pose _middlePinchPose = Pose.identity;

			private Pose _pinchPose = Pose.identity;
		}
	}
}
