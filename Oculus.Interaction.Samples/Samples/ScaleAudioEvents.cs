using System;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction.Samples
{
	public class ScaleAudioEvents : MonoBehaviour
	{
		public UnityEvent WhenScalingStarted
		{
			get
			{
				return this._whenScalingStarted;
			}
		}

		public UnityEvent WhenScalingEnded
		{
			get
			{
				return this._whenScalingEnded;
			}
		}

		public UnityEvent WhenScaledUp
		{
			get
			{
				return this._whenScaledUp;
			}
		}

		public UnityEvent WhenScaledDown
		{
			get
			{
				return this._whenScaledDown;
			}
		}

		private Transform TrackedTransform
		{
			get
			{
				if (!(this._trackedTransform == null))
				{
					return this._trackedTransform;
				}
				return base.transform;
			}
		}

		private void ScalingStarted()
		{
			this._lastStep = this.TrackedTransform.localScale;
			this._whenScalingStarted.Invoke();
		}

		private void ScalingEnded()
		{
			this._whenScalingEnded.Invoke();
		}

		private float GetTotalDelta(out ScaleAudioEvents.Direction direction)
		{
			float magnitude = this._lastStep.magnitude;
			float magnitude2 = this.TrackedTransform.localScale.magnitude;
			if (magnitude2 == magnitude)
			{
				direction = ScaleAudioEvents.Direction.None;
			}
			else
			{
				direction = ((magnitude2 > magnitude) ? ScaleAudioEvents.Direction.ScaleUp : ScaleAudioEvents.Direction.ScaleDown);
			}
			if (direction != ScaleAudioEvents.Direction.ScaleUp)
			{
				return magnitude - magnitude2;
			}
			return magnitude2 - magnitude;
		}

		private void UpdateScaling()
		{
			if (this._stepSize <= 0f || this._maxEventFreq <= 0)
			{
				return;
			}
			float stepSize = this._stepSize;
			if (this.GetTotalDelta(out this._direction) > stepSize)
			{
				this._lastStep = this.TrackedTransform.localScale;
				if (Time.time - this._lastEventTime >= 1f / (float)this._maxEventFreq)
				{
					this._lastEventTime = Time.time;
					if (this._direction == ScaleAudioEvents.Direction.ScaleUp)
					{
						this._whenScaledUp.Invoke();
						return;
					}
					this._whenScaledDown.Invoke();
				}
			}
		}

		protected virtual void Awake()
		{
			this.InteractableView = (this._interactableView as IInteractableView);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void Update()
		{
			bool isScaling = this._isScaling;
			this._isScaling = (this.InteractableView.State == InteractableState.Select);
			if (!this._isScaling)
			{
				if (isScaling)
				{
					this.ScalingEnded();
					return;
				}
			}
			else
			{
				if (!isScaling)
				{
					this.ScalingStarted();
				}
				this.UpdateScaling();
			}
		}

		[SerializeField]
		[Interface(typeof(IInteractableView), new Type[]
		{

		})]
		private Object _interactableView;

		[Tooltip("Transform to track scale of. If not provided, transform of this component is used.")]
		[SerializeField]
		[Optional]
		private Transform _trackedTransform;

		[Tooltip("The increase in scale magnitude that will fire the step event")]
		[SerializeField]
		private float _stepSize = 0.4f;

		[Tooltip("Events will not be fired more frequently than this many times per second")]
		[SerializeField]
		private int _maxEventFreq = 20;

		[SerializeField]
		private UnityEvent _whenScalingStarted = new UnityEvent();

		[SerializeField]
		private UnityEvent _whenScalingEnded = new UnityEvent();

		[SerializeField]
		private UnityEvent _whenScaledUp = new UnityEvent();

		[SerializeField]
		private UnityEvent _whenScaledDown = new UnityEvent();

		private IInteractableView InteractableView;

		private bool _isScaling;

		private Vector3 _lastStep;

		private float _lastEventTime;

		private ScaleAudioEvents.Direction _direction;

		protected bool _started;

		private enum Direction
		{
			None,
			ScaleUp,
			ScaleDown
		}
	}
}
