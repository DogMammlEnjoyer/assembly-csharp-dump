using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ControllerSelector : MonoBehaviour, ISelector
	{
		public ControllerButtonUsage ControllerButtonUsage
		{
			get
			{
				return this._controllerButtonUsage;
			}
			set
			{
				this._controllerButtonUsage = value;
			}
		}

		public ControllerSelector.ControllerSelectorLogicOperator RequireButtonUsages
		{
			get
			{
				return this._requireButtonUsages;
			}
			set
			{
				this._requireButtonUsages = value;
			}
		}

		public IController Controller { get; private set; }

		public event Action WhenSelected = delegate()
		{
		};

		public event Action WhenUnselected = delegate()
		{
		};

		protected virtual void Awake()
		{
			this.Controller = (this._controller as IController);
		}

		protected virtual void Start()
		{
		}

		protected virtual void Update()
		{
			if ((this._requireButtonUsages == ControllerSelector.ControllerSelectorLogicOperator.All) ? this.Controller.IsButtonUsageAllActive(this._controllerButtonUsage) : this.Controller.IsButtonUsageAnyActive(this._controllerButtonUsage))
			{
				if (this._selected)
				{
					return;
				}
				this._selected = true;
				this.WhenSelected();
				return;
			}
			else
			{
				if (!this._selected)
				{
					return;
				}
				this._selected = false;
				this.WhenUnselected();
				return;
			}
		}

		public void InjectAllControllerSelector(IController controller)
		{
			this.InjectController(controller);
		}

		public void InjectController(IController controller)
		{
			this._controller = (controller as Object);
			this.Controller = controller;
		}

		[Tooltip("The controller to check.")]
		[SerializeField]
		[Interface(typeof(IController), new Type[]
		{

		})]
		private Object _controller;

		[Tooltip("The buttons to check.")]
		[SerializeField]
		private ControllerButtonUsage _controllerButtonUsage;

		[Tooltip("Determines how many of the checked buttons must be pressed for the controller to be selecting. 'All' requires all of the buttons to be pressed. 'Any' requires only one to be pressed.")]
		[SerializeField]
		private ControllerSelector.ControllerSelectorLogicOperator _requireButtonUsages;

		private bool _selected;

		public enum ControllerSelectorLogicOperator
		{
			Any,
			All
		}
	}
}
