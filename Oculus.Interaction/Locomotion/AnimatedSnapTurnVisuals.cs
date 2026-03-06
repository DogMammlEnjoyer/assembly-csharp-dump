using System;
using System.Collections;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class AnimatedSnapTurnVisuals : MonoBehaviour, ITimeConsumer
	{
		private ILocomotionEventBroadcaster LocomotionEventBroadcaster { get; set; }

		public AnimationCurve Animation
		{
			get
			{
				return this._animation;
			}
			set
			{
				this._animation = value;
			}
		}

		public float HighlightOffset
		{
			get
			{
				return this._highlightOffset;
			}
			set
			{
				this._highlightOffset = value;
			}
		}

		public void SetTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
		}

		protected virtual void Awake()
		{
			this.LocomotionEventBroadcaster = (this._locomotionEventBroadcaster as ILocomotionEventBroadcaster);
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
				this.LocomotionEventBroadcaster.WhenLocomotionPerformed += this.HandleLocomotionPerformed;
				this._visuals.Progress = 0f;
				this._visuals.Value = 0f;
				this._visuals.HighLight = false;
				this._visuals.UpdateVisual();
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.LocomotionEventBroadcaster.WhenLocomotionPerformed -= this.HandleLocomotionPerformed;
			}
		}

		private void HandleLocomotionPerformed(LocomotionEvent ev)
		{
			if (ev.Rotation == LocomotionEvent.RotationType.Relative)
			{
				this.StopAnimation();
				float direction = (Mathf.Repeat(ev.Pose.rotation.eulerAngles.y, 360f) < 180f) ? 1f : -1f;
				this._animationRoutine = base.StartCoroutine(this.AnimationRoutine(direction));
			}
		}

		private void StopAnimation()
		{
			if (this._animationRoutine != null)
			{
				base.StopCoroutine(this._animationRoutine);
				this._animationRoutine = null;
			}
		}

		private IEnumerator AnimationRoutine(float direction)
		{
			float totalTime = this._animation.keys[this._animation.keys.Length - 1].time;
			float startTime = this._timeProvider();
			float ellapsedTime = 0f;
			this._visuals.Progress = 0f;
			this._visuals.Value = direction;
			this._visuals.HighLight = false;
			this._visuals.UpdateVisual();
			while (ellapsedTime < totalTime)
			{
				this._visuals.Progress = this._animation.Evaluate(ellapsedTime);
				this._visuals.HighLight = (this._progressValue > 0.8f);
				ellapsedTime = this._timeProvider() - startTime;
				this._visuals.UpdateVisual();
				yield return null;
			}
			this._visuals.Progress = 0f;
			this._visuals.Value = 0f;
			this._visuals.HighLight = false;
			this._visuals.UpdateVisual();
			yield break;
		}

		public void InjectAllAnimatedSnapTurnVisuals(TurnArrowVisuals visuals, ILocomotionEventBroadcaster locomotionEventBroadcaster)
		{
			this.InjectVisuals(visuals);
			this.InjectLocomotionEventBroadcaster(locomotionEventBroadcaster);
		}

		public void InjectVisuals(TurnArrowVisuals visuals)
		{
			this._visuals = visuals;
		}

		public void InjectLocomotionEventBroadcaster(ILocomotionEventBroadcaster locomotionEventBroadcaster)
		{
			this.LocomotionEventBroadcaster = locomotionEventBroadcaster;
			this._locomotionEventBroadcaster = (locomotionEventBroadcaster as Object);
		}

		[SerializeField]
		private TurnArrowVisuals _visuals;

		[SerializeField]
		[Interface(typeof(ILocomotionEventBroadcaster), new Type[]
		{

		})]
		private Object _locomotionEventBroadcaster;

		[SerializeField]
		private AnimationCurve _animation;

		[SerializeField]
		private float _highlightOffset = 0.8f;

		private Func<float> _timeProvider = () => Time.time;

		private float _progressValue;

		private Coroutine _animationRoutine;

		protected bool _started;
	}
}
