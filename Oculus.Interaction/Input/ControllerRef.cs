using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class ControllerRef : MonoBehaviour, IController, IActiveState
	{
		protected virtual void Awake()
		{
			this.Controller = (this._controller as IController);
		}

		protected virtual void Start()
		{
		}

		public Handedness Handedness
		{
			get
			{
				return this.Controller.Handedness;
			}
		}

		public bool IsConnected
		{
			get
			{
				return this.Controller.IsConnected;
			}
		}

		public bool IsPoseValid
		{
			get
			{
				return this.Controller.IsPoseValid;
			}
		}

		public ControllerInput ControllerInput
		{
			get
			{
				return this.Controller.ControllerInput;
			}
		}

		public event Action WhenUpdated
		{
			add
			{
				this.Controller.WhenUpdated += value;
			}
			remove
			{
				this.Controller.WhenUpdated -= value;
			}
		}

		public bool Active
		{
			get
			{
				return this.IsConnected;
			}
		}

		public bool TryGetPose(out Pose pose)
		{
			return this.Controller.TryGetPose(out pose);
		}

		public bool TryGetPointerPose(out Pose pose)
		{
			return this.Controller.TryGetPointerPose(out pose);
		}

		public float Scale
		{
			get
			{
				return this.Controller.Scale;
			}
		}

		public bool IsButtonUsageAnyActive(ControllerButtonUsage buttonUsage)
		{
			return this.Controller.IsButtonUsageAnyActive(buttonUsage);
		}

		public bool IsButtonUsageAllActive(ControllerButtonUsage buttonUsage)
		{
			return this.Controller.IsButtonUsageAllActive(buttonUsage);
		}

		public void InjectAllControllerRef(IController controller)
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

		private IController Controller;
	}
}
