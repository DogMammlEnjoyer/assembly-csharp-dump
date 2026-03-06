using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class ControllerButtonUsageActiveState : MonoBehaviour, IActiveState
	{
		public bool Active
		{
			get
			{
				return this.Controller.IsButtonUsageAnyActive(this._controllerButtonUsage);
			}
		}

		protected virtual void Awake()
		{
			this.Controller = (this._controller as IController);
		}

		protected virtual void Start()
		{
		}

		public void InjectAllControllerButtonUsageActiveState(IController controller, ControllerButtonUsage controllerButtonUsage)
		{
			this.InjectController(controller);
			this.InjectControllerButtonUsage(controllerButtonUsage);
		}

		public void InjectController(IController controller)
		{
			this._controller = (controller as Object);
			this.Controller = controller;
		}

		public void InjectControllerButtonUsage(ControllerButtonUsage controllerButtonUsage)
		{
			this._controllerButtonUsage = controllerButtonUsage;
		}

		[SerializeField]
		[Interface(typeof(IController), new Type[]
		{

		})]
		private Object _controller;

		private IController Controller;

		[SerializeField]
		private ControllerButtonUsage _controllerButtonUsage;
	}
}
