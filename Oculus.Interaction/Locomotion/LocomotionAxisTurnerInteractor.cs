using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class LocomotionAxisTurnerInteractor : Interactor<LocomotionAxisTurnerInteractor, LocomotionAxisTurnerInteractable>, IAxis1D
	{
		public float DeadZone
		{
			get
			{
				return this._deadZone;
			}
			set
			{
				this._deadZone = value;
			}
		}

		public override bool ShouldHover
		{
			get
			{
				return Mathf.Abs(this._horizontalAxisValue) > this._deadZone;
			}
		}

		public override bool ShouldUnhover
		{
			get
			{
				return !this.ShouldHover;
			}
		}

		protected override bool ComputeShouldSelect()
		{
			return this.ShouldHover;
		}

		protected override bool ComputeShouldUnselect()
		{
			return this.ShouldUnhover;
		}

		protected override void Awake()
		{
			base.Awake();
			this.Axis2D = (this._axis2D as IAxis2D);
		}

		protected override void OnDisable()
		{
			if (this._started)
			{
				this._horizontalAxisValue = 0f;
			}
			base.OnDisable();
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this.EndStart(ref this._started);
		}

		protected override void DoPreprocess()
		{
			base.DoPreprocess();
			this._horizontalAxisValue = this.Axis2D.Value().x;
		}

		protected override LocomotionAxisTurnerInteractable ComputeCandidate()
		{
			return null;
		}

		public float Value()
		{
			return this._horizontalAxisValue;
		}

		public void InjectAllLocomotionAxisTurner(IAxis2D axis2D)
		{
			this.InjectAxis2D(axis2D);
		}

		public void InjectAxis2D(IAxis2D axis2D)
		{
			this._axis2D = (axis2D as Object);
			this.Axis2D = axis2D;
		}

		[SerializeField]
		[Interface(typeof(IAxis2D), new Type[]
		{

		})]
		[Tooltip("Input 2D Axis from which the Horizontal axis will be extracted")]
		private Object _axis2D;

		private IAxis2D Axis2D;

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("The Axis.x absolute value must be bigger than this to go into Hover and Select states")]
		private float _deadZone = 0.5f;

		private float _horizontalAxisValue;
	}
}
