using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class UseFingerControllerAPI : MonoBehaviour, IFingerUseAPI
	{
		private IController Controller { get; set; }

		protected virtual void Awake()
		{
			this.Controller = (this._controller as IController);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		public float GetFingerUseStrength(HandFinger finger)
		{
			switch (finger)
			{
			case HandFinger.Thumb:
				return Mathf.Max(this.Controller.ControllerInput.Trigger, this.Controller.ControllerInput.Grip);
			case HandFinger.Index:
				return this.Controller.ControllerInput.Trigger;
			case HandFinger.Middle:
				return this.Controller.ControllerInput.Grip;
			default:
				return 0f;
			}
		}

		public void InjectAllUseFingerRawPinchAPI(IController controller)
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

		protected bool _started;
	}
}
