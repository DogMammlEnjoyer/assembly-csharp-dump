using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oculus.Interaction.PoseDetection.Debug;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	public class SequenceActiveState : MonoBehaviour, IActiveState
	{
		protected virtual void Start()
		{
		}

		public bool Active
		{
			get
			{
				return (this._activateIfStepsStarted && this._sequence.CurrentActivationStep > 0 && !this._sequence.Active) || (this._activateIfStepsComplete && this._sequence.Active);
			}
		}

		public void InjectAllSequenceActiveState(Sequence sequence, bool activateIfStepsStarted, bool activateIfStepsComplete)
		{
			this.InjectSequence(sequence);
			this.InjectActivateIfStepsStarted(activateIfStepsStarted);
			this.InjectActivateIfStepsComplete(activateIfStepsComplete);
		}

		public void InjectSequence(Sequence sequence)
		{
			this._sequence = sequence;
		}

		public void InjectActivateIfStepsStarted(bool activateIfStepsStarted)
		{
			this._activateIfStepsStarted = activateIfStepsStarted;
		}

		public void InjectActivateIfStepsComplete(bool activateIfStepsComplete)
		{
			this._activateIfStepsComplete = activateIfStepsComplete;
		}

		[Tooltip("The Sequence that will drive this component.")]
		[SerializeField]
		private Sequence _sequence;

		[Tooltip("If true, this ActiveState will become Active as soon as the first sequence step becomes Active.")]
		[SerializeField]
		private bool _activateIfStepsStarted;

		[Tooltip("If true, this ActiveState will be active when the supplied Sequence is Active.")]
		[SerializeField]
		private bool _activateIfStepsComplete = true;

		private class DebugModel : ActiveStateModel<SequenceActiveState>
		{
			protected override Task<IEnumerable<IActiveState>> GetChildrenAsync(SequenceActiveState activeState)
			{
				return Task.FromResult<IEnumerable<IActiveState>>(new Sequence[]
				{
					activeState._sequence
				});
			}
		}
	}
}
