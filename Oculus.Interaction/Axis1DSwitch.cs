using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class Axis1DSwitch : MonoBehaviour, IAxis1D
	{
		protected IAxis1D Current
		{
			get
			{
				if (!this.ActiveState.Active)
				{
					return this.AxisWhenInactive;
				}
				return this.AxisWhenActive;
			}
		}

		protected virtual void Awake()
		{
			this.ActiveState = (this._activeState as IActiveState);
			this.AxisWhenActive = (this._axisWhenActive as IAxis1D);
			this.AxisWhenInactive = (this._axisWhenInactive as IAxis1D);
		}

		protected virtual void Start()
		{
		}

		public float Value()
		{
			return this.Current.Value();
		}

		public void InjectAllAxis1DSwitch(IActiveState activeState, IAxis1D axisWhenActive, IAxis1D axisWhenInactive)
		{
			this.InjectActiveState(activeState);
			this.InjectAxisWhenActive(axisWhenActive);
			this.InjectAxisWhenInactive(axisWhenInactive);
		}

		public void InjectActiveState(IActiveState activeState)
		{
			this._activeState = (activeState as Object);
			this.ActiveState = activeState;
		}

		private void InjectAxisWhenActive(IAxis1D axisWhenActive)
		{
			this.AxisWhenActive = axisWhenActive;
			this._axisWhenActive = (axisWhenActive as Object);
		}

		private void InjectAxisWhenInactive(IAxis1D axisWhenInactive)
		{
			this.AxisWhenInactive = axisWhenInactive;
			this._axisWhenInactive = (axisWhenInactive as Object);
		}

		[SerializeField]
		[Interface(typeof(IActiveState), new Type[]
		{

		})]
		private Object _activeState;

		private IActiveState ActiveState;

		[SerializeField]
		[Interface(typeof(IAxis1D), new Type[]
		{

		})]
		private Object _axisWhenActive;

		[SerializeField]
		[Interface(typeof(IAxis1D), new Type[]
		{

		})]
		private Object _axisWhenInactive;

		protected IAxis1D AxisWhenActive;

		protected IAxis1D AxisWhenInactive;
	}
}
