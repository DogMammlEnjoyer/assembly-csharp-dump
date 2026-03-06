using System;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class LocomotionTunneling : MonoBehaviour, ITimeConsumer, IDeltaTimeConsumer
	{
		private ILocomotionEventHandler Locomotor { get; set; }

		public AnimationCurve RotationStrength
		{
			get
			{
				return this._rotationStrength;
			}
			set
			{
				this._rotationStrength = value;
			}
		}

		public AnimationCurve AccelerationStrength
		{
			get
			{
				return this._accelerationStrength;
			}
			set
			{
				this._accelerationStrength = value;
			}
		}

		public AnimationCurve MovementStrength
		{
			get
			{
				return this._movementStrength;
			}
			set
			{
				this._movementStrength = value;
			}
		}

		public float FadeOutTime
		{
			get
			{
				return this._fadeOutTime;
			}
			set
			{
				this._fadeOutTime = value;
			}
		}

		public float FadeOutWait
		{
			get
			{
				return this._fadeOutWait;
			}
			set
			{
				this._fadeOutWait = value;
			}
		}

		public void SetDeltaTimeProvider(Func<float> deltaTimeProvider)
		{
			this._deltaTimeProvider = deltaTimeProvider;
		}

		public void SetTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
		}

		protected virtual void Awake()
		{
			this.Locomotor = (this._locomotor as ILocomotionEventHandler);
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
				this._tunneling.UseAimingTarget = false;
				this._tunneling.UserFOV = 360f;
				this._lastVelocity = Vector3.zero;
				this.Locomotor.WhenLocomotionEventHandled += this.HandleLocomotionEventHandled;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._tunneling.enabled = false;
				this.Locomotor.WhenLocomotionEventHandled -= this.HandleLocomotionEventHandled;
			}
		}

		private void HandleLocomotionEventHandled(LocomotionEvent locomotionEvent, Pose pose)
		{
			if (locomotionEvent.Rotation == LocomotionEvent.RotationType.Velocity)
			{
				float f;
				Vector3 vector;
				locomotionEvent.Pose.rotation.ToAngleAxis(out f, out vector);
				float fov = this._rotationStrength.Evaluate(Mathf.Abs(f));
				this.SetFOV(fov);
			}
			if (locomotionEvent.Translation == LocomotionEvent.TranslationType.Velocity)
			{
				Vector3 position = pose.position;
				Vector3 vector = position - this._lastVelocity;
				float time = vector.magnitude / this._deltaTimeProvider();
				this._lastVelocity = position;
				float b = this._movementStrength.Evaluate(position.magnitude);
				float a = this._accelerationStrength.Evaluate(time);
				this.SetFOV(Mathf.Min(a, b));
			}
			if (locomotionEvent.Translation == LocomotionEvent.TranslationType.Relative || locomotionEvent.Translation == LocomotionEvent.TranslationType.Absolute || locomotionEvent.Translation == LocomotionEvent.TranslationType.AbsoluteEyeLevel)
			{
				float fov2 = this._movementStrength.Evaluate(pose.position.magnitude);
				this.SetFOV(fov2);
			}
		}

		private void SetFOV(float fov)
		{
			if (Mathf.Approximately(fov, 0f))
			{
				return;
			}
			this._tunneling.enabled = true;
			this._tunneling.UserFOV = Mathf.Min(this._tunneling.UserFOV, fov);
			this._tunneling.AlphaStrength = 1f;
			this._fadeOutStart = this._timeProvider();
		}

		protected virtual void LateUpdate()
		{
			float num = this._timeProvider() - this._fadeOutStart;
			if (num < this._fadeOutWait)
			{
				return;
			}
			float num2 = num - this._fadeOutWait;
			float num3 = Mathf.Lerp(1f, 0f, num2 / this._fadeOutTime);
			this._tunneling.AlphaStrength = num3;
			if (num3 <= 0f)
			{
				this._tunneling.enabled = false;
				this._tunneling.UserFOV = 360f;
			}
		}

		[SerializeField]
		[Interface(typeof(ILocomotionEventHandler), new Type[]
		{

		})]
		private Object _locomotor;

		[SerializeField]
		private TunnelingEffect _tunneling;

		[SerializeField]
		private AnimationCurve _rotationStrength;

		[SerializeField]
		private AnimationCurve _accelerationStrength;

		[SerializeField]
		private AnimationCurve _movementStrength;

		[SerializeField]
		private float _fadeOutTime = 0.2f;

		[SerializeField]
		private float _fadeOutWait = 0.2f;

		private Func<float> _deltaTimeProvider = () => Time.deltaTime;

		private Func<float> _timeProvider = () => Time.time;

		private bool _started;

		private Vector3 _lastVelocity = Vector3.zero;

		private float _fadeOutStart;
	}
}
