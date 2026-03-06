using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oculus.Interaction.PoseDetection.Debug;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	public class Sequence : MonoBehaviour, IActiveState, ITimeConsumer
	{
		public void SetTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
		}

		private IActiveState RemainActiveWhile { get; set; }

		public int CurrentActivationStep { get; private set; }

		protected virtual void Awake()
		{
			this.RemainActiveWhile = (this._remainActiveWhile as IActiveState);
			this.ResetState();
		}

		protected virtual void Start()
		{
			if (this._stepsToActivate == null)
			{
				this._stepsToActivate = Array.Empty<Sequence.ActivationStep>();
			}
			Sequence.ActivationStep[] stepsToActivate = this._stepsToActivate;
			for (int i = 0; i < stepsToActivate.Length; i++)
			{
				stepsToActivate[i].Start();
			}
		}

		protected virtual void Update()
		{
			float num = this._timeProvider();
			if (this.Active)
			{
				bool flag = this.RemainActiveWhile != null && this.RemainActiveWhile.Active;
				if (!flag)
				{
					if (this._wasRemainActive)
					{
						this._cooldownExceededTime = num + this._remainActiveCooldown;
					}
					if (this._cooldownExceededTime <= num)
					{
						this.Active = false;
					}
				}
				this._wasRemainActive = flag;
				if (!this.Active)
				{
					this.ResetState();
				}
				return;
			}
			if (this.CurrentActivationStep < this._stepsToActivate.Length)
			{
				Sequence.ActivationStep activationStep = this._stepsToActivate[this.CurrentActivationStep];
				if (num > this._stepFailedTime && this.CurrentActivationStep > 0 && activationStep.MaxStepTime > 0f)
				{
					this.ResetState();
				}
				bool active = activationStep.ActiveState.Active;
				if (active && !this._currentStepWasActive)
				{
					this._currentStepActivatedTime = num + activationStep.MinActiveTime;
				}
				if (num >= this._currentStepActivatedTime && this._currentStepWasActive)
				{
					int num2 = this.CurrentActivationStep + 1;
					bool flag2 = !active;
					bool flag3 = num2 == this._stepsToActivate.Length || this._stepsToActivate[num2].ActiveState.Active;
					if (flag2 || flag3)
					{
						this.EnterNextStep(num);
					}
				}
				this._currentStepWasActive = active;
				return;
			}
			if (this.RemainActiveWhile != null)
			{
				this.Active = this.RemainActiveWhile.Active;
			}
		}

		private void EnterNextStep(float time)
		{
			int currentActivationStep = this.CurrentActivationStep;
			this.CurrentActivationStep = currentActivationStep + 1;
			this._currentStepWasActive = false;
			if (this.CurrentActivationStep < this._stepsToActivate.Length)
			{
				Sequence.ActivationStep activationStep = this._stepsToActivate[this.CurrentActivationStep];
				this._stepFailedTime = time + activationStep.MaxStepTime;
				return;
			}
			this.Active = true;
			this._cooldownExceededTime = time + this._remainActiveCooldown;
			NativeMethods.isdk_NativeComponent_Activate(6009334026819888500UL);
		}

		private void ResetState()
		{
			this.CurrentActivationStep = 0;
			this._currentStepWasActive = false;
			this._currentStepActivatedTime = 0f;
		}

		public bool Active { get; private set; }

		public void InjectOptionalStepsToActivate(Sequence.ActivationStep[] stepsToActivate)
		{
			this._stepsToActivate = stepsToActivate;
		}

		public void InjectOptionalRemainActiveWhile(IActiveState activeState)
		{
			this._remainActiveWhile = (activeState as Object);
			this.RemainActiveWhile = activeState;
		}

		[Obsolete("Use SetTimeProvider()")]
		public void InjectOptionalTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
		}

		[Tooltip("The sequence will step through these ActivationSteps one at a time, advancing when each step becomes Active. Once all steps are active, the sequence itself will become Active.")]
		[SerializeField]
		[Optional]
		private Sequence.ActivationStep[] _stepsToActivate;

		[Tooltip("Once the sequence is active, it will remain active as long as this IActiveState is Active.")]
		[SerializeField]
		[Optional]
		[Interface(typeof(IActiveState), new Type[]
		{

		})]
		private Object _remainActiveWhile;

		[Tooltip("Sequence will not become inactive until RemainActiveWhile has been inactive for at least this many seconds.")]
		[SerializeField]
		[Optional]
		private float _remainActiveCooldown;

		private Func<float> _timeProvider = () => Time.time;

		private float _currentStepActivatedTime;

		private float _stepFailedTime;

		private bool _currentStepWasActive;

		private float _cooldownExceededTime;

		private bool _wasRemainActive;

		[Serializable]
		public class ActivationStep
		{
			public IActiveState ActiveState { get; private set; }

			public float MinActiveTime
			{
				get
				{
					return this._minActiveTime;
				}
			}

			public float MaxStepTime
			{
				get
				{
					return this._maxStepTime;
				}
			}

			public ActivationStep()
			{
			}

			public ActivationStep(IActiveState activeState, float minActiveTime, float maxStepTime)
			{
				this.ActiveState = activeState;
				this._minActiveTime = minActiveTime;
				this._maxStepTime = maxStepTime;
			}

			public void Start()
			{
				if (this.ActiveState == null)
				{
					this.ActiveState = (this._activeState as IActiveState);
				}
			}

			[Tooltip("The IActiveState that is used to determine if the conditions of this step are fulfilled.")]
			[SerializeField]
			[Interface(typeof(IActiveState), new Type[]
			{

			})]
			private Object _activeState;

			[SerializeField]
			[Tooltip("This step must be consistently active for this amount of time before continuing to the next step.")]
			private float _minActiveTime;

			[SerializeField]
			[Tooltip("Maximum time that can be spent waiting for this step to complete, before the whole sequence is abandoned. This value must be greater than minActiveTime, or zero. This value is ignored if zero, and for the first step in the list.")]
			private float _maxStepTime;
		}

		private class DebugModel : ActiveStateModel<Sequence>
		{
			private IEnumerator GetChildrenCoroutine(Sequence sequence, TaskCompletionSource<IEnumerable<IActiveState>> tcs)
			{
				for (;;)
				{
					if (!sequence._stepsToActivate.Any((Sequence.ActivationStep s) => s.ActiveState == null))
					{
						break;
					}
					yield return null;
				}
				List<IActiveState> list = new List<IActiveState>();
				list.AddRange(from step in sequence._stepsToActivate
				select step.ActiveState);
				list.Add(sequence.RemainActiveWhile);
				tcs.SetResult(from c in list
				where c != null
				select c);
				yield break;
			}

			protected override Task<IEnumerable<IActiveState>> GetChildrenAsync(Sequence activeState)
			{
				TaskCompletionSource<IEnumerable<IActiveState>> taskCompletionSource = new TaskCompletionSource<IEnumerable<IActiveState>>();
				if (activeState.isActiveAndEnabled)
				{
					activeState.StartCoroutine(this.GetChildrenCoroutine(activeState, taskCompletionSource));
				}
				else
				{
					taskCompletionSource.SetResult(Enumerable.Empty<IActiveState>());
				}
				return taskCompletionSource.Task;
			}
		}
	}
}
