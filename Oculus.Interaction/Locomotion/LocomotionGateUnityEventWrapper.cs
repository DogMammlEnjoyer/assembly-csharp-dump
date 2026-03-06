using System;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction.Locomotion
{
	public class LocomotionGateUnityEventWrapper : MonoBehaviour
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
				this._locomotionGate.WhenActiveModeChanged += this.HandleActiveModeChanged;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._locomotionGate.WhenActiveModeChanged -= this.HandleActiveModeChanged;
			}
		}

		private void HandleActiveModeChanged(LocomotionGate.LocomotionModeEventArgs locomotionModeArgs)
		{
			if (locomotionModeArgs.PreviousMode == LocomotionGate.LocomotionMode.None)
			{
				this.WhenEnterLocomotion.Invoke();
				return;
			}
			if (locomotionModeArgs.NewMode == LocomotionGate.LocomotionMode.Teleport)
			{
				this.WhenChangedToTeleport.Invoke();
				return;
			}
			if (locomotionModeArgs.NewMode == LocomotionGate.LocomotionMode.Turn)
			{
				this.WhenChangedToTurn.Invoke();
				return;
			}
			if (locomotionModeArgs.NewMode == LocomotionGate.LocomotionMode.None)
			{
				this.WhenExitLocomotion.Invoke();
			}
		}

		public void InjectAllLocomotionGateUnityEventWrapper(LocomotionGate locomotionGate)
		{
			this.InjectLocomotionGate(locomotionGate);
		}

		public void InjectLocomotionGate(LocomotionGate locomotionGate)
		{
			this._locomotionGate = locomotionGate;
		}

		[SerializeField]
		private LocomotionGate _locomotionGate;

		public UnityEvent WhenEnterLocomotion;

		public UnityEvent WhenExitLocomotion;

		public UnityEvent WhenChangedToTurn;

		public UnityEvent WhenChangedToTeleport;

		protected bool _started;
	}
}
