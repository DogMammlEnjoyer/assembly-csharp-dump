using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ControllerActiveState : MonoBehaviour, IActiveState
	{
		public bool Active
		{
			get
			{
				return this.Controller.IsConnected;
			}
		}

		protected virtual void Awake()
		{
			this.Controller = (this._controller as IController);
		}

		protected virtual void Start()
		{
		}

		public void InjectAllControllerActiveState(IController controller)
		{
			this.InjectController(controller);
		}

		public void InjectController(IController controller)
		{
			this._controller = (controller as Object);
			this.Controller = controller;
		}

		[Tooltip("ActiveState will be true while this controller is connected.")]
		[SerializeField]
		[Interface(typeof(IController), new Type[]
		{

		})]
		private Object _controller;

		private IController Controller;
	}
}
