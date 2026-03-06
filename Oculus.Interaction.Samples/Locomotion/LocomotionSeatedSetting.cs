using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Oculus.Interaction.Locomotion
{
	public class LocomotionSeatedSetting : MonoBehaviour
	{
		public float SeatedHeightOffset
		{
			get
			{
				return this._seatedHeightOffset;
			}
			set
			{
				this._seatedHeightOffset = value;
			}
		}

		protected void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this._seated.onValueChanged.AddListener(new UnityAction<bool>(this.HandleSeatedChanged));
				this._standing.onValueChanged.AddListener(new UnityAction<bool>(this.HandleStandingChanged));
				if (this._standing.isOn)
				{
					this.HandleStandingChanged(true);
					return;
				}
				this.HandleSeatedChanged(true);
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._seated.onValueChanged.RemoveListener(new UnityAction<bool>(this.HandleSeatedChanged));
				this._standing.onValueChanged.RemoveListener(new UnityAction<bool>(this.HandleStandingChanged));
			}
		}

		private void HandleSeatedChanged(bool seated)
		{
			if (seated)
			{
				this._locomotor.HeightOffset = this._seatedHeightOffset;
			}
		}

		private void HandleStandingChanged(bool standing)
		{
			if (standing)
			{
				this._locomotor.HeightOffset = 0f;
			}
		}

		public void InjectAllSeatedMode(Toggle seated, Toggle standing, FirstPersonLocomotor locomotor)
		{
			this.InjectSeated(seated);
			this.InjectStanding(standing);
			this.InjectLocomotor(locomotor);
		}

		public void InjectSeated(Toggle seated)
		{
			this._seated = seated;
		}

		public void InjectStanding(Toggle standing)
		{
			this._standing = standing;
		}

		public void InjectLocomotor(FirstPersonLocomotor locomotor)
		{
			this._locomotor = locomotor;
		}

		[SerializeField]
		private Toggle _seated;

		[SerializeField]
		private Toggle _standing;

		[SerializeField]
		private FirstPersonLocomotor _locomotor;

		[SerializeField]
		private float _seatedHeightOffset = 0.5f;

		protected bool _started;
	}
}
