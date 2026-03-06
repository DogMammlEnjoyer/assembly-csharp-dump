using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Oculus.Interaction.Locomotion
{
	public class LocomotionComfortVignetteSetting : MonoBehaviour
	{
		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this._toggle.onValueChanged.AddListener(new UnityAction<bool>(this.InjectCurve));
				this.InjectCurve(this._toggle.isOn);
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.InjectCurve));
			}
		}

		private void InjectCurve(bool inject)
		{
			if (!inject)
			{
				return;
			}
			switch (this._comfortType)
			{
			case LocomotionComfortVignetteSetting.ComfortType.Turning:
				this._tunneling.RotationStrength = this._curve;
				return;
			case LocomotionComfortVignetteSetting.ComfortType.Accelerating:
				this._tunneling.AccelerationStrength = this._curve;
				return;
			case LocomotionComfortVignetteSetting.ComfortType.Moving:
				this._tunneling.MovementStrength = this._curve;
				return;
			default:
				return;
			}
		}

		public void InjectAllComfortOption(LocomotionComfortVignetteSetting.ComfortType comfortType, Toggle toggle, AnimationCurve curve, LocomotionTunneling tunneling)
		{
			this.InjectComfortType(comfortType);
			this.InjectToggle(toggle);
			this.InjectCurve(curve);
			this.InjectTunneling(tunneling);
		}

		public void InjectComfortType(LocomotionComfortVignetteSetting.ComfortType comfortType)
		{
			this._comfortType = comfortType;
		}

		public void InjectToggle(Toggle toggle)
		{
			this._toggle = toggle;
		}

		public void InjectCurve(AnimationCurve curve)
		{
			this._curve = curve;
		}

		public void InjectTunneling(LocomotionTunneling tunneling)
		{
			this._tunneling = tunneling;
		}

		[SerializeField]
		private Toggle _toggle;

		[SerializeField]
		private LocomotionComfortVignetteSetting.ComfortType _comfortType;

		[SerializeField]
		private AnimationCurve _curve;

		[SerializeField]
		private LocomotionTunneling _tunneling;

		protected bool _started;

		public enum ComfortType
		{
			Turning,
			Accelerating,
			Moving
		}
	}
}
