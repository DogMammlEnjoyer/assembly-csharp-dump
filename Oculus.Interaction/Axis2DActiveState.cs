using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class Axis2DActiveState : MonoBehaviour, IActiveState
	{
		private IAxis2D InputAxis { get; set; }

		public Axis2DActiveState.CheckComponent CheckAxis
		{
			get
			{
				return this._checkAxis;
			}
			set
			{
				this._checkAxis = value;
			}
		}

		public Axis2DActiveState.ComparisonMode Comparison
		{
			get
			{
				return this._comparison;
			}
			set
			{
				this._comparison = value;
			}
		}

		public bool AbsoluteValues
		{
			get
			{
				return this._absoluteValues;
			}
			set
			{
				this._absoluteValues = value;
			}
		}

		public bool Active { get; private set; }

		protected virtual void Awake()
		{
			this.InputAxis = (this._inputAxis as IAxis2D);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Active = false;
			}
		}

		protected virtual void Update()
		{
			this.HandleValueUpdated(this.InputAxis.Value());
		}

		private void HandleValueUpdated(Vector2 value)
		{
			if (this.AbsoluteValues)
			{
				value.x = Mathf.Abs(value.x);
				value.y = Mathf.Abs(value.y);
			}
			this.Active = ((this.Comparison == Axis2DActiveState.ComparisonMode.GreaterThan) ? this.CheckGreaterThan(value) : this.CheckLessThan(value));
		}

		private bool CheckGreaterThan(Vector2 value)
		{
			if (this.CheckAxis == Axis2DActiveState.CheckComponent.X)
			{
				return value.x > this._thresold.x;
			}
			if (this.CheckAxis == Axis2DActiveState.CheckComponent.Y)
			{
				return value.y > this._thresold.y;
			}
			if (this.CheckAxis != Axis2DActiveState.CheckComponent.Any)
			{
				return this.CheckAxis == Axis2DActiveState.CheckComponent.All && value.y > this._thresold.y && value.x > this._thresold.x;
			}
			return value.y > this._thresold.y || value.x > this._thresold.x;
		}

		private bool CheckLessThan(Vector2 value)
		{
			if (this.CheckAxis == Axis2DActiveState.CheckComponent.X)
			{
				return value.x < this._thresold.x;
			}
			if (this.CheckAxis == Axis2DActiveState.CheckComponent.Y)
			{
				return value.y < this._thresold.y;
			}
			if (this.CheckAxis != Axis2DActiveState.CheckComponent.Any)
			{
				return this.CheckAxis == Axis2DActiveState.CheckComponent.All && value.y < this._thresold.y && value.x < this._thresold.x;
			}
			return value.y < this._thresold.y || value.x < this._thresold.x;
		}

		[SerializeField]
		[Interface(typeof(IAxis2D), new Type[]
		{

		})]
		private Object _inputAxis;

		[SerializeField]
		private Axis2DActiveState.CheckComponent _checkAxis = Axis2DActiveState.CheckComponent.Y;

		[SerializeField]
		private Axis2DActiveState.ComparisonMode _comparison;

		[SerializeField]
		private bool _absoluteValues;

		[SerializeField]
		private Vector2 _thresold = new Vector2(0f, 0.5f);

		protected bool _started;

		public enum CheckComponent
		{
			Any,
			X,
			Y,
			All
		}

		public enum ComparisonMode
		{
			GreaterThan,
			LessThan
		}
	}
}
