using System;
using Meta.XR.Util;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	[Feature(Feature.Interaction)]
	public class OVRButtonAxis1D : MonoBehaviour, IAxis1D
	{
		public float NearValue
		{
			get
			{
				return this._nearValue;
			}
			set
			{
				this._nearValue = value;
			}
		}

		public float TouchValue
		{
			get
			{
				return this._touchValue;
			}
			set
			{
				this._touchValue = value;
			}
		}

		public float ButtonValue
		{
			get
			{
				return this._buttonValue;
			}
			set
			{
				this._buttonValue = value;
			}
		}

		public float Value()
		{
			return this._value;
		}

		private float Target
		{
			get
			{
				if (OVRInput.Get(this._button, this._controller))
				{
					return this._buttonValue;
				}
				if (OVRInput.Get(this._touch, this._controller))
				{
					return this._touchValue;
				}
				if (OVRInput.Get(this._near, this._controller))
				{
					return this._nearValue;
				}
				return 0f;
			}
		}

		protected virtual void Update()
		{
			float target = this.Target;
			if (this._currentTarget != target)
			{
				this._baseValue = this._value;
				this._currentTarget = target;
				this._curve.Start();
			}
			this._value = this._curve.Progress() * (this._currentTarget - this._baseValue);
		}

		public void InjectAllOVRButtonAxis1D(OVRInput.Controller controller, OVRInput.Button near, OVRInput.Button touch, OVRInput.Button button)
		{
			this._controller = controller;
			this._near = near;
			this._touch = touch;
			this._button = button;
		}

		public void InjectOptionalCurve(ProgressCurve progressCurve)
		{
			this._curve = progressCurve;
		}

		[SerializeField]
		private OVRInput.Controller _controller;

		[SerializeField]
		private OVRInput.Button _near;

		[SerializeField]
		private OVRInput.Button _touch;

		[SerializeField]
		private OVRInput.Button _button;

		[SerializeField]
		private float _nearValue = 0.1f;

		[SerializeField]
		private float _touchValue = 0.5f;

		[SerializeField]
		private float _buttonValue = 1f;

		[SerializeField]
		private ProgressCurve _curve = new ProgressCurve(AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), 0.1f);

		private float _baseValue;

		private float _value;

		private float _currentTarget;
	}
}
