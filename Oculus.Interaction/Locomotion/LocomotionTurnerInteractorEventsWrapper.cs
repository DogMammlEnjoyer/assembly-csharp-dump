using System;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction.Locomotion
{
	public class LocomotionTurnerInteractorEventsWrapper : MonoBehaviour
	{
		public UnityEvent WhenTurnDirectionLeft
		{
			get
			{
				return this._whenTurnDirectionLeft;
			}
		}

		public UnityEvent WhenTurnDirectionRight
		{
			get
			{
				return this._whenTurnDirectionRight;
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this._turner.WhenTurnDirectionChanged += this.HandleTurnDirectionChanged;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._turner.WhenTurnDirectionChanged -= this.HandleTurnDirectionChanged;
			}
		}

		private void HandleTurnDirectionChanged(float dir)
		{
			if (dir > 0f)
			{
				this._whenTurnDirectionLeft.Invoke();
				return;
			}
			if (dir < 0f)
			{
				this._whenTurnDirectionRight.Invoke();
			}
		}

		public void InjectAllLocomotionTurnerInteractorEventsWrapper(LocomotionTurnerInteractor turner)
		{
			this.InjectTurner(turner);
		}

		public void InjectTurner(LocomotionTurnerInteractor turner)
		{
			this._turner = turner;
		}

		[SerializeField]
		private LocomotionTurnerInteractor _turner;

		[SerializeField]
		private UnityEvent _whenTurnDirectionLeft;

		[SerializeField]
		private UnityEvent _whenTurnDirectionRight;

		protected bool _started;
	}
}
