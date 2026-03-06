using System;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction.Samples
{
	public class RotationAudioEvents : MonoBehaviour
	{
		public UnityEvent WhenRotationStarted
		{
			get
			{
				return this._whenRotationStarted;
			}
		}

		public UnityEvent WhenRotationEnded
		{
			get
			{
				return this._whenRotationEnded;
			}
		}

		public UnityEvent WhenRotatedOpen
		{
			get
			{
				return this._whenRotatedOpen;
			}
		}

		public UnityEvent WhenRotatedClosed
		{
			get
			{
				return this._whenRotatedClosed;
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

		private void RotationStarted()
		{
			this._baseDelta = this.GetTotalDelta();
			this._lastCrossedDirection = RotationAudioEvents.Direction.None;
			this._whenRotationStarted.Invoke();
		}

		private void RotationEnded()
		{
			this._whenRotationEnded.Invoke();
		}

		private Quaternion GetCurrentRotation()
		{
			return Quaternion.Inverse(this._relativeTo.rotation) * this.TrackedTransform.rotation;
		}

		private float GetTotalDelta()
		{
			return Quaternion.Angle(this._relativeTo.rotation, this.GetCurrentRotation());
		}

		private void UpdateRotation()
		{
			float totalDelta = this.GetTotalDelta();
			if (totalDelta > this._maxRangeDeg)
			{
				return;
			}
			if (Mathf.Abs(totalDelta - this._baseDelta) > this._thresholdDeg)
			{
				RotationAudioEvents.Direction direction = (totalDelta - this._baseDelta > 0f) ? RotationAudioEvents.Direction.Opening : RotationAudioEvents.Direction.Closing;
				if (direction != this._lastCrossedDirection)
				{
					this._lastCrossedDirection = direction;
					if (direction == RotationAudioEvents.Direction.Opening)
					{
						this._whenRotatedOpen.Invoke();
					}
					else
					{
						this._whenRotatedClosed.Invoke();
					}
				}
			}
			if (this._lastCrossedDirection == RotationAudioEvents.Direction.Opening)
			{
				this._baseDelta = Mathf.Max(this._baseDelta, totalDelta);
				return;
			}
			if (this._lastCrossedDirection == RotationAudioEvents.Direction.Closing)
			{
				this._baseDelta = Mathf.Min(this._baseDelta, totalDelta);
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
			bool isRotating = this._isRotating;
			this._isRotating = (this.InteractableView.State == InteractableState.Select);
			if (!this._isRotating)
			{
				if (isRotating)
				{
					this.RotationEnded();
					return;
				}
			}
			else
			{
				if (!isRotating)
				{
					this.RotationStarted();
				}
				this.UpdateRotation();
			}
		}

		[SerializeField]
		[Interface(typeof(IInteractableView), new Type[]
		{

		})]
		private Object _interactableView;

		[Tooltip("Transform to track rotation of. If not provided, transform of this component is used.")]
		[SerializeField]
		[Optional]
		private Transform _trackedTransform;

		[SerializeField]
		private Transform _relativeTo;

		[Tooltip("The angle delta at which the threshold crossed event will be fired.")]
		[SerializeField]
		private float _thresholdDeg = 20f;

		[Tooltip("Maximum rotation arc within which the crossed event will be triggered.")]
		[SerializeField]
		[Range(1f, 150f)]
		private float _maxRangeDeg = 150f;

		[SerializeField]
		private UnityEvent _whenRotationStarted = new UnityEvent();

		[SerializeField]
		private UnityEvent _whenRotationEnded = new UnityEvent();

		[SerializeField]
		private UnityEvent _whenRotatedOpen = new UnityEvent();

		[SerializeField]
		private UnityEvent _whenRotatedClosed = new UnityEvent();

		private IInteractableView InteractableView;

		private float _baseDelta;

		private bool _isRotating;

		private RotationAudioEvents.Direction _lastCrossedDirection;

		protected bool _started;

		private enum Direction
		{
			None,
			Opening,
			Closing
		}
	}
}
