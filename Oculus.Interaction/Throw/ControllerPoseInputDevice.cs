using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Throw
{
	public class ControllerPoseInputDevice : MonoBehaviour, IPoseInputDevice
	{
		public IController Controller { get; private set; }

		public bool IsInputValid
		{
			get
			{
				return this.Controller.IsConnected && this.Controller.IsPoseValid;
			}
		}

		public bool IsHighConfidence
		{
			get
			{
				return this.IsInputValid;
			}
		}

		public bool GetRootPose(out Pose pose)
		{
			pose = Pose.identity;
			return this.IsInputValid && this.Controller.TryGetPose(out pose);
		}

		protected virtual void Awake()
		{
			this.Controller = (this._controller as IController);
		}

		protected virtual void Start()
		{
		}

		public ValueTuple<Vector3, Vector3> GetExternalVelocities()
		{
			return new ValueTuple<Vector3, Vector3>(Vector3.zero, Vector3.zero);
		}

		public void InjectAllControllerPoseInputDevice(IController controller)
		{
			this.InjectController(controller);
		}

		public void InjectController(IController controller)
		{
			this._controller = (controller as Object);
			this.Controller = controller;
		}

		[SerializeField]
		[Interface(typeof(IController), new Type[]
		{

		})]
		private Object _controller;
	}
}
