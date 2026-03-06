using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class Axis1DPrioritySelector : MonoBehaviour, IAxis1D
	{
		protected IAxis1D Current
		{
			get
			{
				return this.GetActiveAxis();
			}
		}

		protected virtual void Awake()
		{
			Axis1DPrioritySelector.AxisData[] axisData = this._axisData;
			for (int i = 0; i < axisData.Length; i++)
			{
				axisData[i].Initialize();
			}
			this.FallbackIfNoMatchAxis = (this._fallbackIfNoMatchAxis as IAxis1D);
		}

		protected virtual void Start()
		{
			Axis1DPrioritySelector.AxisData[] axisData = this._axisData;
			for (int i = 0; i < axisData.Length; i++)
			{
				axisData[i].Validate(this);
			}
		}

		public float Value()
		{
			return this.Current.Value();
		}

		private IAxis1D GetActiveAxis()
		{
			if (this.ActiveAxis != null && this.ActiveAxis.ActiveState.Active)
			{
				return this.ActiveAxis.Axis;
			}
			foreach (Axis1DPrioritySelector.AxisData axisData2 in this._axisData)
			{
				if (axisData2.ActiveState.Active)
				{
					this.ActiveAxis = axisData2;
					return this.ActiveAxis.Axis;
				}
			}
			return this.FallbackIfNoMatchAxis;
		}

		public void InjectAll(Axis1DPrioritySelector.AxisData[] axisData, IAxis1D fallbackIfNoMatchAxis)
		{
			this._axisData = axisData;
			for (int i = 0; i < axisData.Length; i++)
			{
				axisData[i].Validate(this);
			}
			this.FallbackIfNoMatchAxis = fallbackIfNoMatchAxis;
			this._fallbackIfNoMatchAxis = (fallbackIfNoMatchAxis as Object);
		}

		[SerializeField]
		private Axis1DPrioritySelector.AxisData[] _axisData;

		[SerializeField]
		[Interface(typeof(IAxis1D), new Type[]
		{

		})]
		private Object _fallbackIfNoMatchAxis;

		private IAxis1D FallbackIfNoMatchAxis;

		private Axis1DPrioritySelector.AxisData ActiveAxis;

		[Serializable]
		public class AxisData
		{
			public void Initialize()
			{
				this.ActiveState = (this._activeState as IActiveState);
				this.Axis = (this._axis as IAxis1D);
			}

			public void Validate(Component context)
			{
			}

			[SerializeField]
			[Interface(typeof(IActiveState), new Type[]
			{

			})]
			private Object _activeState;

			public IActiveState ActiveState;

			[SerializeField]
			[Interface(typeof(IAxis1D), new Type[]
			{

			})]
			private Object _axis;

			public IAxis1D Axis;
		}
	}
}
