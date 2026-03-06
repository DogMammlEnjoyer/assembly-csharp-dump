using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ControllerAxis2D : MonoBehaviour, IAxis2D
	{
		private IController Controller { get; set; }

		public ControllerAxis2DUsage Axis
		{
			get
			{
				return this._axis;
			}
			set
			{
				this._axis = value;
			}
		}

		protected virtual void Awake()
		{
			this.Controller = (this._controller as IController);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		public Vector2 Value()
		{
			Vector2 vector = Vector2.zero;
			if (!this._started)
			{
				return vector;
			}
			if ((this._axis & ControllerAxis2DUsage.Primary2DAxis) != ControllerAxis2DUsage.None)
			{
				vector = this.Controller.ControllerInput.Primary2DAxis;
			}
			if ((this._axis & ControllerAxis2DUsage.Secondary2DAxis) != ControllerAxis2DUsage.None)
			{
				vector += this.Controller.ControllerInput.Secondary2DAxis;
			}
			return vector;
		}

		public void InjectAllControllerAxis2DActiveState(IController controller)
		{
			this.InjectController(controller);
		}

		public void InjectController(IController controller)
		{
			this.Controller = controller;
			this._controller = (controller as Object);
		}

		[SerializeField]
		[Interface(typeof(IController), new Type[]
		{

		})]
		private Object _controller;

		[SerializeField]
		private ControllerAxis2DUsage _axis = ControllerAxis2DUsage.Primary2DAxis;

		protected bool _started;
	}
}
